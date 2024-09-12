using AutoMapper;
using BillProcessorAPI.Dtos;
using BillProcessorAPI.Dtos.BroadcastMessage;
using BillProcessorAPI.Dtos.Common;
using BillProcessorAPI.Dtos.Flutterwave;
using BillProcessorAPI.Entities;
using BillProcessorAPI.Enums;
using BillProcessorAPI.Helpers;
using BillProcessorAPI.Helpers.BroadcastMessage;
using BillProcessorAPI.Helpers.Flutterwave;
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
using AsyncAwaitBestPractices;

namespace BillProcessorAPI.Services.Implementations
{
    public class FlutterwaveService : IFlutterwaveService
    {
        private readonly IRepository<BillTransaction> _billTransactionsRepo;

        private readonly IInvoiceRepository _invoiceRepo;
        private readonly IRepository<BillPayerInfo> _billPayerRepository;
        private readonly IRepository<WebhookNotification> _oldAppWebhook;
        private readonly ICutlyService _cutlyService;

        private readonly FlutterwaveOptions _flutterOptions;
        private readonly IHttpService _httpService;
        private readonly IChargeService _chargeService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _context;
        private ILogger<FlutterwaveService> _logger;
        private readonly BusinessesPhoneNumber _phoneNumberOptions;
        private readonly ReceiptBroadcastConfig _receiptBroadcastOptions;

        public FlutterwaveService(
            IRepository<BillTransaction> billTransactionsRepo,
            IOptions<BillTransactionSettings> settings,
            IRepository<BillPayerInfo> billPayerRepository,
            IOptions<FlutterwaveOptions> flutterOptions,
            IHttpService httpService,
            IChargeService chargeService,
            IInvoiceRepository invoiceRepo,
            IMapper mapper,
            ILogger<FlutterwaveService> logger,
            IHttpContextAccessor context,
            IRepository<WebhookNotification> oldAppWebhook,
            IOptions<BusinessesPhoneNumber> phoneNumberOptions,
            IOptions<ReceiptBroadcastConfig> receiptBroadcastOptions,
            ICutlyService cutlyService)
        {
            _billTransactionsRepo = billTransactionsRepo;
            _billPayerRepository = billPayerRepository;
            _flutterOptions = flutterOptions.Value;
            _httpService = httpService;
            _chargeService = chargeService;
            _invoiceRepo = invoiceRepo;
            _mapper = mapper;
            _logger = logger;
            _context = context;
            _oldAppWebhook = oldAppWebhook;
            _phoneNumberOptions = phoneNumberOptions.Value;
            _receiptBroadcastOptions = receiptBroadcastOptions.Value;
            _cutlyService = cutlyService;
        }

        public async Task<SuccessResponse<PaymentCreationResponse>> CreateTransaction(string email, decimal amount, string billPaymentCode, string phoneNumber)
        {
            if (_flutterOptions == null)
                throw new RestException(HttpStatusCode.BadRequest, "please update your application setting file");

            if (amount < _flutterOptions.MinimumPayableAmount)
            {
                throw new RestException(HttpStatusCode.BadRequest, $"The minimum amount payable is {_flutterOptions.MinimumPayableAmount}");
            }
            if (string.IsNullOrEmpty(email) || amount < 0)
                throw new RestException(HttpStatusCode.BadRequest, "All fields are required");

            var billPayer = await _billPayerRepository.Query(x => x.billCode == billPaymentCode).OrderByDescending(c => c.UpdatedAt).FirstOrDefaultAsync()
                        ?? throw new RestException(HttpStatusCode.NotFound, "unable to fetch bill payer for this transaction");

            IDictionary<string, string> param = new Dictionary<string, string>();
            param.Add(key: "Authorization", _flutterOptions.SecretKey);
            var headerParam = new RequestHeader(param);
            var trxReference = FlutterConfig.PaymentReference();

            var flutterwaveRequestPayload = new FCreateTransactionRequestDto
            {
                Amount = amount,
                Tx_ref = trxReference,
                Currency = "NGN",
                Redirect_url = _flutterOptions.RedirectUrl,
                Customer = new CustomerDto
                {
                    Email = email,
                },
                Meta = new MetaDto
                {
                    Webguid = billPaymentCode,
                    Creditaccount = billPayer.CreditAccount
                }
            };
            
            BillTransaction billTransaction;

            try
            {
                var charge = new ChargesInputDto(amount, "Flutterwave");
                billTransaction = new BillTransaction
                {
                    GatewayType = EGatewayType.Flutterwave,
                    Status = ETransactionStatus.Pending.ToString(),
                    BillPayerInfoId = billPayer.Id,
                    PayerName = billPayer.PayerName,
                    BillNumber = billPayer.billCode,
                    Pid = billPayer.Pid,
                    RevName = billPayer.RevName,
                    PhoneNumber = AppendCountryCode(phoneNumber,"234"),
                    DueDate = billPayer.AcctCloseDate,
                    TransactionReference = trxReference,
                    TransactionCharge = _chargeService.CalculateBillChargesOnAmount(charge).Data.AmountCharge,
                    AmountDue = billPayer.AmountDue,
                    AmountPaid = amount,
                    PaymentInfoRequestData = JsonConvert.SerializeObject(flutterwaveRequestPayload)
                };

                await _billTransactionsRepo.AddAsync(billTransaction);
                await _billTransactionsRepo.SaveChangesAsync();
                
                var url = $"{_flutterOptions.BaseUrl}/{_flutterOptions.CreateTransaction}";

                var paymentCreationResponse = await _httpService
                    .Post<FlutterwaveResponse<LinkData>, FCreateTransactionRequestDto>(url, headerParam, flutterwaveRequestPayload);
                if (paymentCreationResponse.Data.Status != "success")
                    throw new RestException(HttpStatusCode.BadRequest, "An error occured while processing your request");
               
                var billInvoice = new Invoice();

                async Task TransactionCommitAction ()
                {
                    _billTransactionsRepo.Update(billTransaction);
                    billTransaction.PaymentUrl = paymentCreationResponse.Data.Data.Link;
                    billTransaction.PaymentInfoResponseData = JsonConvert.SerializeObject(paymentCreationResponse.Data);
                    billTransaction.PaymentInfoRequestData = JsonConvert.SerializeObject(flutterwaveRequestPayload);
                    // TODO Add error message gotten from api call if it failed.

                    if (paymentCreationResponse.Data.Status == "success")
                    {
                        billInvoice = _mapper.Map<Invoice>(billPayer);
                        billInvoice.BillTransactionId = billTransaction.Id;
                        billInvoice.TransactionReference = trxReference;
                        billInvoice.DueDate = billPayer.AcctCloseDate;
                        billInvoice.BillNumber = billPaymentCode;
                        billInvoice.GatewayType = EGatewayType.Flutterwave;
                        billInvoice.PhoneNumber = billPayer.PhoneNumber;
                        billInvoice.TransactionCharge = _chargeService.CalculateBillChargesOnAmount(charge).Data.AmountCharge;
                    
                        await _invoiceRepo.AddAsync(billInvoice);
                    }
                   
                };
                
                await _invoiceRepo.BeginTransaction(TransactionCommitAction);
                
                var response = new PaymentCreationResponse
                {
                    SystemCharge = billInvoice.TransactionCharge,
                    PayLink = paymentCreationResponse.Data.Data.Link,
                    Status = paymentCreationResponse.Data.Status,
                };

                return new SuccessResponse<PaymentCreationResponse>
                {
                    Data = response,
                    Message = "Payment created successfully",
                };
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

        public async Task<SuccessResponse<string>> PaymentNotification(WebHookNotificationWrapper model)
        {
            BillTransaction transaction = null;
            try
            {
                if (model == null)
                    throw new RestException(HttpStatusCode.BadRequest, "invalid transaction, notification content is null and empty");

                transaction = await _billTransactionsRepo.FirstOrDefault(x => x.TransactionReference == model.TransactionReference);
                
                _logger.LogCritical($"Payment notification from Flutterwave just came in as at: {DateTime.UtcNow}");
                _logger.LogCritical($"Details of notification : {JsonConvert.SerializeObject(model)}");


                if (transaction is null)
                {
                    var webhook = model.ToWebHook();
                    webhook.Data = JsonConvert.SerializeObject(model);
                    webhook.GatewayType = "Flutterwave";
                    webhook.Remark = "This webhook transaction is not found on the billTransaction";
                    // saving the webhook to the database since no transaction was retrieved for the webhook to update
                    await _oldAppWebhook.AddAsync(webhook);
                    await _oldAppWebhook.SaveChangesAsync();
                    return new SuccessResponse<string>
                    {
                        Data = "Transaction Completed"
                    };
                }
                _logger.LogInformation($"No transaction was found for the webhook received, webhook saved to the database");
                transaction.NotificationReceiptTime = DateTime.Now;

                transaction.ReceiptUrl = model.ReceiptNumber;
                await _billTransactionsRepo.SaveChangesAsync();

                //verify transaction with flutterwave using the transactionId from the webhook
                IDictionary<string, string> param = new Dictionary<string, string>();
                param.Add(key: "Authorization", _flutterOptions.SecretKey);
                var headerParam = new RequestHeader(param);

                var url = $"{_flutterOptions.BaseUrl}/{_flutterOptions.VerifyByReference}/?tx_ref={model.TransactionReference}";
                var verificationResponse = await _httpService.Get<FlutterwaveResponse<FlutterwaveResponseData>>(url, headerParam);

                if (verificationResponse.Data.Status != "success")
                    _logger.LogCritical($"Unable to verify payment notification from Flutterwave as at: {DateTime.UtcNow}");

                transaction.AmountPaid = verificationResponse.Data.Data.amount;
                transaction.PrinciPalAmount = verificationResponse.Data.Data.amount; // amount paid minus charges
                transaction.Channel = verificationResponse.Data.Data.payment_type;
                transaction.TransactionCharge = transaction.TransactionCharge;
                transaction.GatewayTransactionCharge = (decimal)verificationResponse.Data.Data.app_fee;
                transaction.GatewayTransactionReference = verificationResponse.Data.Data.flw_ref;
                transaction.PaymentReference = model.PaymentRef.ToString();
                transaction.FiName = "N/A";
                transaction.Narration = verificationResponse.Data.Data.narration;

                if (verificationResponse?.Data?.Data.status?.ToUpper() == "SUCCESSFUL")
                {
                    transaction.Status = ETransactionStatus.Successful.ToString();                    
                    ReceiptBroadcast.SendReceipt(transaction, _phoneNumberOptions,_cutlyService,
                        _receiptBroadcastOptions,_httpService).SafeFireAndForget(onException: exception =>
                    {
                        _logger.LogError($"Error occurred on sending receipt {exception}");
                    }, continueOnCapturedContext: true);
                }
                else
                {
                    transaction.Status = ETransactionStatus.Unsuccessful.ToString();
                }
                
                transaction.DateCompleted = verificationResponse?.Data?.Data?.created_at.ToString();
                transaction.StatusMessage = verificationResponse.Data.Data.status;
                transaction.SuccessIndicator = verificationResponse.Data.Data.status;
                transaction.Hash = "N/A";
                transaction.UpdatedAt = DateTime.UtcNow;
                transaction.NotificationResponseData = JsonConvert.SerializeObject(model);
                transaction.Email = verificationResponse.Data.Data.customer.email;
                transaction.isReceiptSent = true;

                await _billTransactionsRepo.SaveChangesAsync();

                //add the receipt to the invoice
                var invoice = await _invoiceRepo.FirstOrDefault(x => x.BillTransactionId == transaction.Id);
                if (invoice is not null)
                {
                    invoice.ReceiptUrl = model.ReceiptNumber;
                    invoice.AmountPaid = verificationResponse?.Data?.Data?.amount ?? 0.0m;
                    invoice.AmountDue = transaction.AmountDue;
                    invoice.GatewayTransactionCharge = (decimal)verificationResponse.Data.Data.app_fee;
                    invoice.UpdatedAt = DateTime.UtcNow;
                    invoice.GatewayTransactionReference = verificationResponse.Data.Data.flw_ref;
                }
              
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred on receipt of payment notification from flutterwave: {ex.Message}", ex);

                if(transaction is not null) transaction.ErrorMessage = ex.ToString();
                throw;
            }
            finally
            {
                if (transaction is not null)
                    await _billTransactionsRepo.SaveChangesAsync();
            }


            return new SuccessResponse<string>
            {
                Data = "Transaction Completed"
            };
        }

        public async Task<SuccessResponse<PaymentConfirmationResponse>> PaymentConfirmation(string status, string tx_ref, string transaction_id)
        {
            var trxStatus = new SuccessResponse<string>();
            if (string.IsNullOrEmpty(status) || string.IsNullOrEmpty(tx_ref))
                throw new RestException(HttpStatusCode.BadGateway, "bad request, status param and transaction reference cannot be null");

            var invoiceResponse = new SuccessResponse<PaymentConfirmationResponse>();
            
            try
            {
                var billTransaction = await _billTransactionsRepo.FirstOrDefault(x => x.TransactionReference == tx_ref);
                if (billTransaction == null)
                    throw new RestException(HttpStatusCode.NotFound, "Unable to fetch transaction: transaction failed");

                // check the transaction for the bill
                // if it's pending dont perform verification..
                if (billTransaction.Status.Equals(ETransactionStatus.Pending.ToString())
                    || billTransaction.Status.Equals(ETransactionStatus.Created.ToString()))
                {
                    var response = _mapper.Map<PaymentConfirmationResponse>(billTransaction);

                    return new SuccessResponse<PaymentConfirmationResponse>
                    {
                        Data = response,
                        Success = false,
                        Message = "Transaction not completed",
                    };
                }

                var verifyPayment = await VerifyTransaction(tx_ref);
                if (!verifyPayment)
                {
                    invoiceResponse.Success = false;
                    invoiceResponse.Message = "Unable to fetch transaction: transaction failed";
                    invoiceResponse.Data = null;
                    
                    // send email of the transaction not being successful and should be resolved in 24hrs.
                    var failureMessage = $"Dear bill payer {billTransaction.PayerName}, {Environment.NewLine} {Environment.NewLine} " +
                                         $"We are unable to successfully complete your transaction at this time." +
                                         $" {Environment.NewLine} Kindly be patient as this transaction will be resolved within 24hrs." +
                                         $"{Environment.NewLine} Thank you for your patience. Transaction Reference Number : {billTransaction.TransactionReference} ";
                    
                    ReceiptBroadcast.SendReceipt(billTransaction, _phoneNumberOptions,_cutlyService,
                        _receiptBroadcastOptions,_httpService, failureMessage).SafeFireAndForget(onException: exception =>
                    {
                        _logger.LogError($"Error occurred on sending receipt  {exception}");
                    }, continueOnCapturedContext: true);
                    
                    return invoiceResponse;
                }

                var invoice = await _invoiceRepo.Query(x => x.BillTransactionId == billTransaction.Id)
                    .Include(x => x.Receipts).FirstOrDefaultAsync();
                if (invoice is null)
                {
                    // create invoice here. as far as the transaction exist on our system. You should not throw exception here
                    throw new RestException(HttpStatusCode.NotFound, "Unable to retrieve invoice for this transaction");
                }

                var invoiceDto = _mapper.Map<PaymentConfirmationResponse>(invoice);
                invoiceDto.DateCompleted = billTransaction.DateCompleted;

                invoiceResponse.Data = invoiceDto;
                invoiceResponse.Success = true;
                invoiceResponse.Message = "Transaction Successful";

                
                //Send customer receipt if receipt is available and still
                if (!billTransaction.isReceiptSent && string.IsNullOrEmpty(billTransaction.ReceiptUrl))
                {
                    ReceiptBroadcast.SendReceipt(billTransaction, _phoneNumberOptions,_cutlyService,
                        _receiptBroadcastOptions,_httpService).SafeFireAndForget(onException: exception =>
                    {
                        _logger.LogError($"Error occurred on sending receipt  {exception}");
                    }, continueOnCapturedContext: true);

                    billTransaction.isReceiptSent = true;
                    await _billTransactionsRepo.SaveChangesAsync();
                }
               
                return invoiceResponse;

            }
            catch (Exception ex)
            {

                throw new RestException(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public async Task<bool> VerifyTransaction(string transactionReference)
        {
            var response = true;
            IDictionary<string, string> param = new Dictionary<string, string>();
            param.Add(key: "Authorization", _flutterOptions.SecretKey);

            var headerParam = new RequestHeader(param);
            var billTransactionRecord = await _billTransactionsRepo.FirstOrDefault(x => x.TransactionReference == transactionReference);
            var url = $"{_flutterOptions.BaseUrl}/{_flutterOptions.VerifyByReference}/?tx_ref={transactionReference}";

            var transaction = await _httpService.Get<FlutterwaveResponse<FlutterwaveResponseData>>(url, headerParam);
            if (transaction.Data.Status != "success")
                throw new RestException(HttpStatusCode.BadRequest, "Unable to fetch transaction for this reference");

            if (transaction.Data.Data.status != "successful"
                || transaction.Data.Data.amount != billTransactionRecord.AmountPaid
                || transaction.Data.Data.currency != "NGN")
            {
                response = false;
            }

            return response;
        }

        public async Task<FailedWebhookResponseModel> ResendWebhook(FailedWebhookRequest model)
        {
            var request = _context.HttpContext.Request;
            if (!request.Headers.ContainsKey(_flutterOptions.ResendWebhookHeader)
                || request.Headers[_flutterOptions.ResendWebhookHeader] != _flutterOptions.ResendWebhookHeaderValue)
            {
                throw new RestException(HttpStatusCode.Unauthorized, "Authorization failed");
            }

            var response = new FailedWebhookResponseModel();
            IDictionary<string, string> resendWehookUrl = new Dictionary<string, string>();
            resendWehookUrl.Add(key: "Authorization", _flutterOptions.SecretKey);
            var headerParamm = new RequestHeader(resendWehookUrl);

            //var billTransationRecord = await _billTransactionsRepo.FirstOrDefault(x => x.PaymentReference == model.PaymentReference.ToString());
            var url = $"{_flutterOptions.BaseUrl}/{_flutterOptions.ResendFailedWebhook}/{model.PaymentReference}/resend-hook";

            try
            {
                var notificationResponse = await _httpService
                       .Post<FailedWebhookResponseModel, FailedWebhookRequest>(url, headerParamm, model);
                if (notificationResponse.Data.Status != "success")
                    throw new RestException(HttpStatusCode.BadRequest, "Unable to resend webhook notification for the payment reference provided");

                response.Status = notificationResponse.Data.Status;
                response.Message = notificationResponse.Data.Message;
                response.Data = notificationResponse.Data.Data;

                return response;

            }
            catch (Exception ex)
            {

                throw new RestException(HttpStatusCode.InternalServerError, ex.Message);
            }

        }

        private string AppendCountryCode(string phoneNumber, string countryCode)
        {
            if (phoneNumber.StartsWith("0"))
            {
                phoneNumber = phoneNumber.Substring(1);
            }
            else
            {
                return $"{countryCode}{phoneNumber}";
            }

            return $"{countryCode}{phoneNumber}";
        }

    }
}
