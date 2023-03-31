using System.Net.Http;
using System.Net;
using BillProcessorAPI.Dtos;
using BillProcessorAPI.Entities;
using BillProcessorAPI.Helpers;
using BillProcessorAPI.Helpers.Revpay;
using BillProcessorAPI.Services.Interfaces;
using Domain.Common;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Tokens;
using AutoMapper;
using BillProcessorAPI.Repositories.Interfaces;
using BillProcessorAPI.Helpers.Paythru;
using Microsoft.Extensions.Options;
using Amazon.Runtime.Internal.Transform;
using Infrastructure.Http;
using Application.DTOs;
using System;
using BillProcessorAPI.Enums;
using Domain.Exceptions;
using BillProcessorAPI.Dtos.Paythru;
using BillProcessorAPI.Dtos.Common;
using System.Runtime.InteropServices;

namespace BillProcessorAPI.Services.Implementations
{
    public class PayThruService : IPayThruService
    {
        private readonly IRepository<BillPayerInfo> _billPayerRepo;
        private readonly IRepository<BillTransaction> _billTransactionsRepo;
        private readonly IRepository<Invoice> _invoiceRepo;
        private readonly IRepository<Receipt> _receiptRepo;
        private readonly PaythruOptions PaythruOptions;
        private readonly IHttpService _httpService;
        private readonly IConfigurationService _configService;
        private readonly IMapper _mapper;
        private ILogger<PayThruService> logger;

        public PayThruService(IRepository<BillPayerInfo> billPayerRepo,
            IRepository<BillTransaction> billTransactions,
            IOptions<PaythruOptions> paythruOptions,
            IHttpService httpService, IConfigurationService configService,
            IMapper mapper,
            IRepository<Invoice> invoiceRepo,
            IRepository<Receipt> receiptRepo,
            ILogger<PayThruService> logger)
        {

            _billPayerRepo = billPayerRepo;
            _billTransactionsRepo = billTransactions;
            PaythruOptions = paythruOptions.Value;
            _httpService = httpService;
            _configService = configService;
            _mapper = mapper;
            _invoiceRepo = invoiceRepo;
            _receiptRepo = receiptRepo;
            this.logger = logger;
        }


        public async Task<SuccessResponse<PaymentCreationResponse>> CreatePayment(int amount, string billCode)
        {

            if (PaythruOptions is null)
            {
                throw new RestException(System.Net.HttpStatusCode.PreconditionFailed, "Kindly configure the required application settings");
            }

            if (amount < PaythruOptions.MinimumPayableAmount)
            {
                throw new RestException(HttpStatusCode.BadRequest, $"The minimum amount payable is {PaythruOptions.MinimumPayableAmount}");
            }
            if (string.IsNullOrEmpty(billCode))
            {
                throw new RestException(HttpStatusCode.BadRequest, "please enter a valid billCode");
            }
          

            try
            {
                // Get the offset from current time in UTC time
                DateTimeOffset dateTimeOffset = new DateTimeOffset(DateTime.Now);
                // Get the unix timestamp in seconds
                string unixTime = dateTimeOffset.ToUnixTimeSeconds().ToString();

                //paythru transaction creation payload
                var paymentCreationPayload = new PayThruPaymentRequestDto
                {
                    amount = amount,
                    productId = PaythruOptions.ProductId,
                    transactionReference = PaythruConfig.GenerateTransactionReference(billCode.ToString()),
                    paymentDescription = "LUC",
                    sign = PaythruConfig.HashForPaythruPaymentCreation(amount.ToString(), PaythruOptions.Secret),
                    paymentType = PaythruOptions.PaymentType
                };

                //login for token generation payload
                var loginTokenPayload = new PaythruLoginRequestDto
                {
                    ApplicationId = PaythruOptions.ApiKey,
                    Password = PaythruConfig.HashForPaythruLoginPassword(PaythruOptions.Secret, unixTime)
                };


                // header parameter for login and token generation
                IDictionary<string, string> loginDict = new Dictionary<string, string>();
                loginDict.Add(key: "timestamp", unixTime);
                var loginHeader = new RequestHeader(loginDict);

                var loginResponse = await _httpService.Post<PaythruLoginResponseDto, PaythruLoginRequestDto>(PaythruOptions.LoginUrl, loginHeader, loginTokenPayload);
                var token = loginResponse.Data.data;

                //paythru authorization header for transation creation
                IDictionary<string, string> trxDict = new Dictionary<string, string>();
                trxDict.Add(key: "authorization", $"paythru {token}");
                //trxDict.Add(key: "ApplicationId", PaythruOptions.ApiKey);
                var trxHeader = new RequestHeader(trxDict);

                var createTransactionResponse = await _httpService.Post<PaythruPaymentResponseDto, PayThruPaymentRequestDto>(PaythruOptions.CreateTransactionUrl, trxHeader, paymentCreationPayload);

                if (createTransactionResponse.Status == 200)
                {

                    var billPayer = await _billPayerRepo.FirstOrDefault(x => x.billCode == billCode)
                        ?? throw new RestException(HttpStatusCode.NotFound, "unable to fetch bill payer for this transaction");
                    var billTransaction = new BillTransaction
                    {
                        GatewayType = EGatewayType.Paythru,
                        Status = ETransactionStatus.Created.ToString(),
                        BillPayerInfoId = billPayer.Id,
                        PayerName = billPayer.PayerName,
                        BillNumber = billPayer.billCode,
                        Pid = billPayer.Pid,
                        RevName = billPayer.RevName,
                        PhoneNumber = billPayer.PhoneNumber,
                        DueDate = billPayer.AcctCloseDate,
                        TransactionReference = paymentCreationPayload.transactionReference,
                        AmountDue = billPayer.AmountDue,
                        Amount = amount,
                        SuccessIndicator = createTransactionResponse.Data.successIndicator,
                        PaymentUrl = createTransactionResponse.Data.payLink,
                        PaymentInfoResponseData = JsonConvert.SerializeObject(createTransactionResponse.Data),
                        PaymentInfoRequestData = JsonConvert.SerializeObject(paymentCreationPayload)
                    };

                    await _billTransactionsRepo.AddAsync(billTransaction);
                    await _billTransactionsRepo.SaveChangesAsync();

                    var chargeModel = new ChargesInputDto
                    {
                        Channel = "Paythru",
                        Amount = paymentCreationPayload.amount
                    };

                    decimal systemChargeCalculation = PaythruOptions.TransactionCharge;
                    createTransactionResponse.Data.systemCharge = systemChargeCalculation;
                    var paymentCreationResponse = new PaymentCreationResponse
                    {
                        PayLink = createTransactionResponse.Data.payLink,
                        SystemCharge = systemChargeCalculation,
                        Status = "success"
                    };

                    return new SuccessResponse<PaymentCreationResponse>
                    {
                        Data = paymentCreationResponse,
                        Message = "Payment created successfully"
                    };

                }
                throw new RestException(HttpStatusCode.BadRequest, $"Invalid request: unable to create transaction");

            }
            catch (HttpException e)
            {
                throw new RestException((HttpStatusCode)e.StatusCode, e.Message);
            }
            catch (Exception ex)
            {
                throw new RestException(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public async Task<SuccessResponse<PaymentVerificationResponseDto>> VerifyPayment(NotificationRequestWrapper transactionNotification)
        {
            logger.LogInformation($"Payment notification from Paythru just came in as at {DateTime.UtcNow}");

            logger.LogDebug(message: $"Details of notification : {transactionNotification.ToString()}");

            if (transactionNotification is null || transactionNotification.TransactionDetails is null)
                throw new RestException(HttpStatusCode.BadRequest, "Transaction notification cannot be null");

            var data = new PaymentVerificationResponseDto();
            bool verificationSuccess = false;
            try
            {

                var billTransaction = await _billTransactionsRepo.FirstOrDefault(x => x.TransactionReference == transactionNotification.TransactionDetails.MerchantReference);
                if (billTransaction == null)
                    throw new RestException(HttpStatusCode.NotFound, "Transaction not found");

                data.TransactionReference = transactionNotification.TransactionDetails.MerchantReference;

                billTransaction.AmountPaid = transactionNotification.TransactionDetails.ResidualAmount;
                billTransaction.PrinciPalAmount = billTransaction.AmountPaid; // amount paid minus charges
                billTransaction.Channel = transactionNotification.TransactionDetails.PaymentMethod;
                billTransaction.TransactionCharge = PaythruOptions.TransactionCharge;
                billTransaction.GatewayTransactionCharge = transactionNotification.TransactionDetails.Commission;
                billTransaction.GatewayTransactionReference = transactionNotification.TransactionDetails.PayThruReference;
                billTransaction.PaymentReference = transactionNotification.TransactionDetails.PaymentReference;
                billTransaction.FiName = transactionNotification.TransactionDetails.FiName;
                billTransaction.Narration = transactionNotification.TransactionDetails.Naration;
                billTransaction.Status = transactionNotification.TransactionDetails.Status;
                billTransaction.DateCompleted = transactionNotification.TransactionDetails.DateCompleted;
                billTransaction.StatusMessage = transactionNotification.TransactionDetails.Status;
                billTransaction.ReceiptUrl = transactionNotification.TransactionDetails.ReceiptUrl;
                billTransaction.SuccessIndicator = transactionNotification.TransactionDetails.ResultCode;
                billTransaction.Hash = transactionNotification.TransactionDetails.Hash;
                billTransaction.NotificationResponseData = JsonConvert.SerializeObject(transactionNotification);


                if (billTransaction.SuccessIndicator != transactionNotification.TransactionDetails.ResultCode)
                {
                    billTransaction.Status = ETransactionStatus.Failed.ToString();
                    billTransaction.StatusMessage = "Transaction failed";
                    data.ResponseCode = ETransactionResponseCodes.Failed;
                    data.Description = "Transaction Failed";

                    await _billTransactionsRepo.SaveChangesAsync();

                    return new SuccessResponse<PaymentVerificationResponseDto>
                    {
                        Data = data,
                        Success = false,
                        Message = "Transaction failed"
                    };
                };


                if (billTransaction.Status == ETransactionStatus.Successful.ToString())
                {
                    verificationSuccess = true;
                    data.ResponseCode = ETransactionResponseCodes.Successful;
                    data.Description = "Transaction Successful";
                }

                //add the receipt to the invoice
                var invoice = await _invoiceRepo.FirstOrDefault(x => x.BillTransactionId == billTransaction.Id);
                if (invoice is null)
                    throw new RestException(HttpStatusCode.NotFound, "No invoice found for this transaction");

                // Create a receipt record
                var receipt = _mapper.Map<Receipt>(billTransaction);
                receipt.TransactionId = billTransaction.Id;
                receipt.PaymentRef = "N/A";
                receipt.InvoiceId = invoice.Id;
                receipt.TransactionDate = billTransaction.DateCompleted;
                receipt.GateWay = billTransaction.GatewayType.ToString();
                receipt.ReceiptUrl = transactionNotification.TransactionDetails.ReceiptUrl;

                await _receiptRepo.AddAsync(receipt);
                await _receiptRepo.SaveChangesAsync();

                return new SuccessResponse<PaymentVerificationResponseDto>
                {
                    Data = data,
                    Success = verificationSuccess
                };

            }
            catch (Exception ex)
            {

                throw new RestException(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public async Task<SuccessResponse<PaymentConfirmationResponse>> ConfirmPayment(ConfirmPaymentRequest model)
        {
            if (string.IsNullOrEmpty(model.Status))
                throw new RestException(HttpStatusCode.BadRequest, "success indicator cannot be null");
            var invoiceResponse = new SuccessResponse<PaymentConfirmationResponse>();
            try
            {
                var transaction = await _billTransactionsRepo.FirstOrDefault(x => x.SuccessIndicator == model.Status);
                if (transaction == null)
                {
                    invoiceResponse.Success = false;
                    invoiceResponse.Message = "Unable to fetch transaction: transaction failed";
                    invoiceResponse.Data = null;
                    return invoiceResponse;
                }

                var invoice = _mapper.Map<PaymentConfirmationResponse>(transaction);

                invoiceResponse.Data = invoice;
                invoiceResponse.Success = true;
                invoiceResponse.Message = "Transaction Successful";

                return invoiceResponse;
            }
            catch (Exception ex)
            {
                throw new RestException(HttpStatusCode.InternalServerError, ex.Message);
            }


        }

    }
}
