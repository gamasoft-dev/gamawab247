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

namespace BillProcessorAPI.Services.Implementations
{
    public class PayThruService : IPayThruService
    {
        private readonly IRepository<BillPayerInfo> _billPayerRepo;
        private readonly IRepository<BillTransaction> _billTransactionsRepo;
        private readonly PaythruOptions PaythruOptions;
        private readonly IHttpService _httpService;
        private readonly IConfigurationService _configService;

        public PayThruService(IRepository<BillPayerInfo> billPayerRepo, IRepository<BillTransaction> billTransactions, IOptions<PaythruOptions> paythruOptions, IHttpService httpService, IConfigurationService configService)
        {

            _billPayerRepo = billPayerRepo;
            _billTransactionsRepo = billTransactions;
            PaythruOptions = paythruOptions.Value;
            _httpService = httpService;
            _configService = configService;
        }

        public  async Task<SuccessResponse<bool>> ConfirmPayment(ConfirmPaymentRequest model)
        {           
            if (string.IsNullOrEmpty(model.Status))
                   throw new RestException(HttpStatusCode.BadRequest, "success indicator cannot be null");

                try
                {
                    var transactionNotification = await _billTransactionsRepo.FirstOrDefault(x => x.SuccessIndicator == model.Status)
                    ?? throw new RestException(HttpStatusCode.NotFound, "An error occured while processing the transactuion");
                    return new SuccessResponse<bool>
                    {
                        Data = true,
                        Message = "Transaction Successful"
                    };
                }
                catch (Exception ex)
                {

                    throw new RestException(HttpStatusCode.InternalServerError, ex.Message);
                }

            
        }

        public async Task<SuccessResponse<PaythruPaymentResponseDto>> CreatePayment(int amount, string billCode)
        {
            if (amount < PaythruOptions.MinimumPayableAmount)
            {
                throw new RestException(HttpStatusCode.BadRequest, $"The minimum amount payable is {PaythruOptions.MinimumPayableAmount}");
            }
            if (string.IsNullOrEmpty(billCode))
            {
                throw new RestException(HttpStatusCode.BadRequest, "please enter a valid billCode");
            }
            if (PaythruOptions is null)
            {
                throw new RestException(System.Net.HttpStatusCode.PreconditionFailed, "Kindly configure the required application settings");
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
                        TransactionReference = paymentCreationPayload.transactionReference,
                        AmountPaid = amount,
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

                    decimal systemChargeCalculation = 100;
                    createTransactionResponse.Data.systemCharge = systemChargeCalculation;

                    return new SuccessResponse<PaythruPaymentResponseDto>
                    {
                        Data = createTransactionResponse.Data,
                        Message = "Data retrieved successfully"
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
            if (transactionNotification is null && transactionNotification.TransactionDetail is null)
                throw new RestException(HttpStatusCode.BadRequest, "Transaction notification cannot be null");
            var data = new PaymentVerificationResponseDto();
            bool verificationSuccess = false;
            try
            {
                var billTransaction = await _billTransactionsRepo.FirstOrDefault(x => x.TransactionReference == transactionNotification.TransactionDetail.PaymentReference);
                if (billTransaction == null)
                    throw new RestException(HttpStatusCode.NotFound, "Transaction not found");

                data.TransactionReference = transactionNotification.TransactionDetail.PaymentReference;

                billTransaction.AmountPaid = transactionNotification.TransactionDetail.Amount;
                billTransaction.Channel = transactionNotification.TransactionDetail.PaymentMethod;
                billTransaction.TransactionCharge = 100;
                billTransaction.GatewayTransactionCharge = transactionNotification.TransactionDetail.Commission;
                billTransaction.GatewayTransactionReference = transactionNotification.TransactionDetail.PayThruReference;
                billTransaction.Narration = transactionNotification.TransactionDetail.Naration;
                billTransaction.Status = ETransactionStatus.Successful.ToString();
                billTransaction.StatusMessage = "Transaction successful";

                if (billTransaction.SuccessIndicator != transactionNotification.TransactionDetail.ResultCode)
                {
                    billTransaction.Status = ETransactionStatus.Failed.ToString();
                    billTransaction.StatusMessage = "Transaction failed";
                    data.ResponseCode = ETransactionResponseCodes.Failed;
                    data.Description = "Transaction Failed";
                };


                if (billTransaction.Status == ETransactionStatus.Successful.ToString())
                {
                    verificationSuccess = true;
                    data.ResponseCode = ETransactionResponseCodes.Successful;
                    data.Description = "Transaction Successful";  
                }

                await _billTransactionsRepo.SaveChangesAsync();
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

       
    }
}
