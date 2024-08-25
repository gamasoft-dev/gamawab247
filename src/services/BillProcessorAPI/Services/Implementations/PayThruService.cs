using Application.DTOs;
using AutoMapper;
using BillProcessorAPI.Dtos;
using BillProcessorAPI.Dtos.BroadcastMessage;
using BillProcessorAPI.Dtos.Common;
using BillProcessorAPI.Dtos.Paythru;
using BillProcessorAPI.Entities;
using BillProcessorAPI.Enums;
using BillProcessorAPI.Helpers;
using BillProcessorAPI.Helpers.BroadcastMessage;
using BillProcessorAPI.Helpers.Paythru;
using BillProcessorAPI.Repositories.Interfaces;
using BillProcessorAPI.Services.Interfaces;
using Domain.Common;
using Domain.Exceptions;
using Infrastructure.Http;
using Infrastructure.ShortLink;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;

namespace BillProcessorAPI.Services.Implementations
{
    public class PayThruService : IPayThruService
    {
        private readonly IRepository<BillPayerInfo> _billPayerRepo;
        private readonly IRepository<BillTransaction> _billTransactionsRepo;
        private readonly IRepository<Invoice> _invoiceRepo;
        private readonly PaythruOptions PaythruOptions;
        private readonly IHttpService _httpService;
        private readonly IConfigurationService _configService;
        private readonly IMapper _mapper;
        private ILogger<PayThruService> _logger;
        private readonly BusinessesPhoneNumber _phoneNumberOptions;
        private readonly ReceiptBroadcastConfig _receiptBroadcastOptions;
        private readonly ICutlyService _cutlyService;
        private readonly ReceiptBroadcastConfig _receiptBroadcastConfig;

        public PayThruService(IRepository<BillPayerInfo> billPayerRepo,
            IRepository<BillTransaction> billTransactions,
            IOptions<PaythruOptions> paythruOptions,
            IHttpService httpService, IConfigurationService configService,
            IMapper mapper,
            IRepository<Invoice> invoiceRepo,
            ILogger<PayThruService> logger, 
            IOptions<BusinessesPhoneNumber> phoneNumberOptions, 
            IOptions<ReceiptBroadcastConfig> receiptBroadcastOptions, 
            ICutlyService cutlyService, IOptions<ReceiptBroadcastConfig> receiptBroadcastConfig)
        {

            _billPayerRepo = billPayerRepo;
            _billTransactionsRepo = billTransactions;
            PaythruOptions = paythruOptions.Value;
            _configService = configService;
            _mapper = mapper;
            _invoiceRepo = invoiceRepo;
            _logger = logger;
            _phoneNumberOptions = phoneNumberOptions.Value;
            _receiptBroadcastOptions = receiptBroadcastOptions.Value;
            _cutlyService = cutlyService;
            _receiptBroadcastConfig = receiptBroadcastConfig.Value;
            _httpService = httpService;
        }


        public async Task<SuccessResponse<PaymentCreationResponse>> CreatePayment(int amount, string billCode)
        {


            if (amount >=0 || billCode.Length >= 0)
            {
                throw new RestException(HttpStatusCode.PreconditionFailed, "This payment option is currently unavailable. Kindly select another payment option.");
            }
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
            if (billCode.Length != 10)
            {
                throw new RestException(HttpStatusCode.BadRequest, "Unrecognized payment code, kindly ensure that your bank payment code is 10 digits and try again.");
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
                        Status = ETransactionStatus.Pending.ToString(),
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

                    _logger.LogInformation($"-------------------------------------------------------------------------");
                    _logger.LogInformation($"Create transaction via Paythru. DateTime : {DateTime.UtcNow}");

                    _logger.LogInformation(message: $"Serialized json value of the transaction :{JsonConvert.SerializeObject(billTransaction)}");

                    _logger.LogInformation($"-------------------------------------------------------------------------");


                    await _billTransactionsRepo.AddAsync(billTransaction);

                    var billInvoice = _mapper.Map<Invoice>(billPayer);
                    billInvoice.BillTransactionId = billTransaction.Id;
                    billInvoice.TransactionReference = billTransaction.TransactionReference;
                    billInvoice.DueDate = billPayer.AcctCloseDate;
                    billInvoice.BillNumber = billTransaction.BillNumber;
                    billInvoice.GatewayType = EGatewayType.Paythru;
                    billInvoice.PhoneNumber = billPayer.PhoneNumber;
                    billInvoice.AmountDue = billTransaction.AmountDue;
                    billInvoice.AmountPaid = billTransaction.AmountPaid;
                    billInvoice.GatewayTransactionCharge = PaythruOptions.TransactionCharge;
                    billInvoice.ReceiptUrl = billTransaction.ReceiptUrl;

                    await _invoiceRepo.AddAsync(billInvoice);
                    _logger.LogInformation("about to save invoice record");
                    await _invoiceRepo.SaveChangesAsync();

                    //var chargeModel = new ChargesInputDto(paymentCreationPayload.amount, "Paythru");

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

        /// <summary>
        /// This is a payemtn notification/ verification service method.
        /// </summary>
        /// <param name="transactionNotification"></param>
        /// <returns></returns>
        /// <exception cref="PaymentVerificationException"></exception>
        /// <exception cref="RestException"></exception>
        public async Task<SuccessResponse<PaymentVerificationResponseDto>> VerifyPayment(dynamic model)
        {
            _logger.LogInformation($"Payment notification from Paythru just came in as at {DateTime.UtcNow}");
            _logger.LogInformation($"-------------------------------------------------------------------------");

            _logger.LogInformation(message: $"Formatted string value of the notification :{model.ToString()}");
            _logger.LogInformation($"-------------------------------------------------------------------------");

            NotificationRequestWrapper transactionNotification = JsonConvert.DeserializeObject<NotificationRequestWrapper>(model.ToString());
            if (transactionNotification is null || transactionNotification.TransactionDetails is null)
                throw new PaymentVerificationException(HttpStatusCode.BadRequest, "Transaction notification cannot be null");

            var data = new PaymentVerificationResponseDto();
            bool verificationSuccess = false;
         

            var billTransaction = await _billTransactionsRepo.FirstOrDefault(x => x.TransactionReference.ToLower().Trim()
            == transactionNotification.TransactionDetails.MerchantReference.ToLower().Trim());

            if (billTransaction == null)
                throw new PaymentVerificationException(HttpStatusCode.NotFound, "Transaction not found");

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
            billTransaction.Email = transactionNotification.TransactionDetails.CustomerInfo.ProvidedEmail;


            if (transactionNotification?.TransactionDetails?.Status?.ToUpper()
                == ETransactionStatus.Successful.ToString().ToUpper())
            {
                billTransaction.Status = ETransactionStatus.Successful.ToString();
            }
            else
            {
                billTransaction.Status = ETransactionStatus.Failed.ToString();
            }

            billTransaction.DateCompleted = transactionNotification.TransactionDetails.DateCompleted;
            billTransaction.StatusMessage = transactionNotification.TransactionDetails.Status;
            billTransaction.ReceiptUrl = transactionNotification.TransactionDetails.ReceiptUrl;
            billTransaction.SuccessIndicator = transactionNotification.TransactionDetails.ResultCode;
            billTransaction.Hash = transactionNotification.TransactionDetails.Hash;
            billTransaction.UpdatedAt = DateTime.UtcNow;
            //billTransaction.NotificationResponseData = JsonConvert.SerializeObject(transactionNotification);
            billTransaction.NotificationResponseData = model.ToString();

            await _billTransactionsRepo.SaveChangesAsync();


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
                
                //Send customer receipt
                await ReceiptBroadcast.SendReceipt(billTransaction, _phoneNumberOptions,
                    _cutlyService, _receiptBroadcastOptions, _httpService);
            }

          
            //add the receipt to the invoice
            var invoice = await _invoiceRepo.FirstOrDefault(x => x.BillTransactionId == billTransaction.Id);
            if (invoice is null)
                throw new PaymentVerificationException(HttpStatusCode.NotFound, "No invoice found for this transaction");

            invoice.ReceiptUrl = transactionNotification.TransactionDetails.ReceiptUrl;
            invoice.AmountPaid = billTransaction.AmountPaid;
            invoice.AmountDue = billTransaction.AmountDue;
            invoice.GatewayTransactionCharge = billTransaction.GatewayTransactionCharge;
            invoice.UpdatedAt = DateTime.UtcNow;
            invoice.GatewayTransactionReference = billTransaction.GatewayTransactionReference;

            // Create a receipt record
            //var receipt = _mapper.Map<Receipt>(billTransaction);
            //receipt.TransactionId = billTransaction.Id;
            //receipt.PaymentRef = billTransaction.TransactionReference;
            //receipt.InvoiceId = invoice.Id;
            //receipt.TransactionDate = billTransaction.DateCompleted;
            //receipt.GateWay = billTransaction.GatewayType.ToString();
            //receipt.ReceiptUrl = transactionNotification.TransactionDetails.ReceiptUrl;

            //await _receiptRepo.AddAsync(receipt);
            //await _receiptRepo.SaveChangesAsync();

            return new SuccessResponse<PaymentVerificationResponseDto>
            {
                Data = data,
                Success = verificationSuccess
            };
            
        }

        public async Task<SuccessResponse<PaymentConfirmationResponse>> ConfirmPayment(ConfirmPaymentRequest model)
        {
            if (string.IsNullOrEmpty(model.Status))
                throw new RestException(HttpStatusCode.BadRequest, "success indicator cannot be null");

            var verificationDto = new SuccessResponse<PaymentConfirmationResponse>();

            try
            {
                var billTransaction = await _billTransactionsRepo.FirstOrDefault(x => x.SuccessIndicator == model.Status);
                if (billTransaction == null)
                    throw new RestException(HttpStatusCode.NotFound, "Unable to fetch transaction: transaction failed");


                // check the transaction for the bill
                // if it's pending dont perform verification..
                if (billTransaction.Status.Equals(ETransactionStatus.Pending.ToString()) ||
                    billTransaction.Status.Equals(ETransactionStatus.Created.ToString()))
                {
                    var response = _mapper.Map<PaymentConfirmationResponse>(billTransaction);

                    return new SuccessResponse<PaymentConfirmationResponse>
                    {
                        Data = response,
                        Success = false,
                        Message = "Transaction not completed",
                    };
                }

                Invoice invoice = await _invoiceRepo.Query(x => x.BillTransactionId == billTransaction.Id)
                    .Include(x => x.Receipts).FirstOrDefaultAsync();
                if (invoice is null)
                    throw new RestException(HttpStatusCode.NotFound, "Unable to retrieve invoice for this transaction");

                var verificationResponse = _mapper.Map<PaymentConfirmationResponse>(invoice);

                verificationDto.Data = verificationResponse;
                verificationDto.Success = true;
                verificationDto.Message = "Transaction Successful";

                return verificationDto;
            }
            catch (Exception ex)
            {
                throw new RestException(HttpStatusCode.InternalServerError, ex.Message);
            }


        }

    }
}
