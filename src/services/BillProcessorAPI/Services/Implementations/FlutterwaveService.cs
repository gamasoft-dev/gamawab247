using AutoMapper;
using BillProcessorAPI.Dtos;
using BillProcessorAPI.Dtos.Common;
using BillProcessorAPI.Dtos.Flutterwave;
using BillProcessorAPI.Entities;
using BillProcessorAPI.Enums;
using BillProcessorAPI.Helpers;
using BillProcessorAPI.Helpers.Flutterwave;
using BillProcessorAPI.Repositories.Interfaces;
using BillProcessorAPI.Services.Interfaces;
using Domain.Common;
using Infrastructure.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;

namespace BillProcessorAPI.Services.Implementations
{
    public class FlutterwaveService : IFlutterwaveService
    {
        private readonly IRepository<BillTransaction> _billTransactionsRepo;
        private readonly IRepository<BillPayerInfo> _billPayerRepository;
        private readonly IRepository<Invoice> _invoiceRepo;
        private readonly FlutterwaveOptions _flutterOptions;
        private readonly IHttpService _httpService;
        private readonly IConfigurationService _configService;
        public FlutterwaveService(
            IRepository<BillTransaction> billTransactionsRepo,
            IOptions<BillTransactionSettings> settings,
            IRepository<BillPayerInfo> billPayerRepository,
            IOptions<FlutterwaveOptions> flutterOptions,
            IHttpService httpService,
            IConfigurationService configService,
            IRepository<Invoice> invoiceRepo)
        {
            _billTransactionsRepo = billTransactionsRepo;
            _billPayerRepository = billPayerRepository;
            _flutterOptions = flutterOptions.Value;
            _httpService = httpService;
            _configService = configService;
            _invoiceRepo = invoiceRepo;
        }


        public async Task<SuccessResponse<PaymentCreationResponse>> CreateTransaction(string email, decimal amount, string billPaymentCode)
        {

            if (string.IsNullOrEmpty(email) || amount < 0)
                throw new RestException(HttpStatusCode.BadRequest, "All fields are required");
            if (_flutterOptions == null)
                throw new RestException(HttpStatusCode.BadRequest, "please update your application setting file");

            IDictionary<string, string> param = new Dictionary<string, string>();
            param.Add(key: "Authorization", _flutterOptions.SecretKey);
            var headerParam = new RequestHeader(param);
            var trxReference = FlutterConfig.PaymentReference();

            var flutterwaveRequestPayload = new FCreateTransactionRequestDto
            {
                Amount = amount,
                Tx_ref = trxReference,
                Currency = "NGN",
                Redirect_url = "https://92d4-102-91-5-91.eu.ngrok.io/flutterwave/payment-confirmation",
                Customer = new CustomerDto
                {
                    Email = email,
                }
            };

            var url = $"{_flutterOptions.BaseUrl}/{_flutterOptions.CreateTransaction}";

            var paymentCreationResponse = await _httpService
                .Post<FlutterwaveResponse<LinkData>, FCreateTransactionRequestDto>(url, headerParam, flutterwaveRequestPayload);
            if (paymentCreationResponse.Data.Status != "success")
                throw new RestException(HttpStatusCode.BadRequest, "An error occured while processing your request");

            var billPayer = await _billPayerRepository.FirstOrDefault(x => x.billCode == billPaymentCode)
                       ?? throw new RestException(HttpStatusCode.NotFound, "unable to fetch bill payer for this transaction");
            var billTransaction = new BillTransaction
            {
                GatewayType = EGatewayType.Flutterwave,
                Status = ETransactionStatus.Created.ToString(),
                TransactionReference = trxReference,
                AmountPaid = amount,
                PaymentUrl = paymentCreationResponse.Data.Data.Link,
                PaymentInfoResponseData = JsonConvert.SerializeObject(paymentCreationResponse.Data),
                PaymentInfoRequestData = JsonConvert.SerializeObject(flutterwaveRequestPayload)
            };

            await _billTransactionsRepo.AddAsync(billTransaction);
            await _billTransactionsRepo.SaveChangesAsync();

            var billInvoice = new Invoice
            {
                AmountDue = billPayer.AmountDue,
                AgencyCode = billPayer.AgencyCode,
                PayerName = billPayer.PayerName,
                RevenueCode = billPayer.RevenueCode,
                OraAgencyRev = billPayer.OraAgencyRev,
                State = billPayer.Status,
                Pid = billPayer.Pid,
                AcctCloseDate = billPayer.AcctCloseDate,
                CbnCode = billPayer.CbnCode,
                AgencyName = billPayer.AgencyName,
                RevName = billPayer.RevName
            };

            await _invoiceRepo.AddAsync(billInvoice);
            await _invoiceRepo.SaveChangesAsync();

            var charge = new ChargesInputDto
            {
                Amount = amount,
                Channel = "Flutterwave"
            };
            var response = new PaymentCreationResponse
            {
                SystemCharge = _configService.CalculateBillChargesOnAmount(charge).Data.AmountCharge,
                PayLink = paymentCreationResponse.Data.Data.Link,
                Status = paymentCreationResponse.Data.Status,
            };

            return new SuccessResponse<PaymentCreationResponse>
            {
                Data = response,
                Message = "Payment created successfully",
            };
           
        }

        public async Task<SuccessResponse<string>> PaymentNotification(string signature, WebHookNotificationWrapper model)
        {
            if (string.IsNullOrEmpty(signature) || (signature != _flutterOptions.Signature))
                throw new RestException(HttpStatusCode.Unauthorized, "unable to verify transaction signature");
            if (model == null)
                throw new RestException(HttpStatusCode.BadRequest, "invalid transaction");

            var transaction = await _billTransactionsRepo.FirstOrDefault(x => x.TransactionReference == model.Data.tx_ref);
            var charge = new ChargesInputDto
            {
                Amount = model.Data.amount,
                Channel = "FlutterWave"
            };

            transaction.GatewayTransactionReference = model.Data.flw_ref;
            transaction.Narration = model.Data.narration;
            transaction.TransactionCharge = _configService.CalculateBillChargesOnAmount(charge).Data.AmountCharge;
            transaction.Status = ETransactionStatus.Successful.ToString();
            transaction.StatusMessage = model.Data.status;
            transaction.AmountPaid = model.Data.amount;
            transaction.Channel = model.Data.payment_type;
            transaction.GatewayTransactionCharge = (decimal)model.Data.app_fee;

            await _billTransactionsRepo.SaveChangesAsync();

            return new SuccessResponse<string>
            {
                Data = "Transaction Completed"
            };
        }

        public async Task<SuccessResponse<string>> PaymentConfirmation(string status, string tx_ref, string transaction_id)
        {
            var trxStatus = new SuccessResponse<string>();
            if (string.IsNullOrEmpty(status) || string.IsNullOrEmpty(tx_ref) || string.IsNullOrEmpty(transaction_id))
                throw new RestException(HttpStatusCode.BadGateway, "bad request");

            var verifyPayment = await VerifyTransaction(tx_ref);
            if (verifyPayment == "Transaction failed")
            {
                trxStatus.Data = "Transaction failed";
                trxStatus.Message = "Failed";
                return trxStatus;
            }

            trxStatus.Data = "Transaction successful";
            trxStatus.Message = "Successful";
            return trxStatus;
        }

        public async Task<string> VerifyTransaction(string transactionReference)
        {
            var response = "Transaction failed";
            IDictionary<string, string> param = new Dictionary<string, string>();
            param.Add(key: "Authorization", _flutterOptions.SecretKey);

            var headerParam = new RequestHeader(param);
            var billTransationRecord = await _billTransactionsRepo.FirstOrDefault(x => x.TransactionReference == transactionReference);
            var url = $"{_flutterOptions.BaseUrl}/{_flutterOptions.VerifyByReference}/?tx_ref={transactionReference}";

            var transaction = await _httpService.Get<FlutterwaveResponse<FlutterwaveResponseData>>(url, headerParam);
            if (transaction.Data.Status != "success")
                throw new RestException(HttpStatusCode.BadRequest, "Unable to fetch transaction for this reference");
            if (transaction.Data.Data.status != "successful"
                || transaction.Data.Data.amount != billTransationRecord.AmountPaid
                || transaction.Data.Data.currency != "NGN")
            {
                return response;
            }

            return response = "Transaction successful";
        }
    }
}
