using AutoMapper;
using BillProcessorAPI.Dtos;
using BillProcessorAPI.Entities;
using BillProcessorAPI.Enums;
using BillProcessorAPI.Helpers;
using BillProcessorAPI.Helpers.Revpay;
using BillProcessorAPI.Repositories.Interfaces;
using BillProcessorAPI.Services.Interfaces;
using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net;

namespace BillProcessorAPI.Services.Implementations
{
	public class BillService : IBillService
	{
		private readonly IConfiguration _config;
		private readonly HttpClient _httpClient;
		private readonly IBillPayerRepository _billPayerRepo;
		private readonly IRepository<BillTransaction> _billTransactionsRepo;
		private readonly IRepository<Receipt> _billReceipt;
		private readonly RevpayOptions RevpayOptions;
		private readonly IMapper _mapper;

        public BillService(IBillPayerRepository billPayerRepo,
            IOptions<RevpayOptions> options,
            IMapper mapper,
            IRepository<BillTransaction> billTransactions,
            HttpClient httpClient,
            IConfiguration config,
            IRepository<Receipt> billReceipt)
        {
            _billPayerRepo = billPayerRepo;
            RevpayOptions = options.Value;
            _mapper = mapper;

            _billTransactionsRepo = billTransactions;
            _httpClient = httpClient;
            _config = config;
            _billReceipt = billReceipt;
        }

        public async Task<SuccessResponse<BillReferenceResponseDto>> ReferenceVerification(string phone, string billPaymentCode)
		{

			if (string.IsNullOrEmpty(billPaymentCode))
			{
				throw new RestException(HttpStatusCode.BadRequest, ResponseMessages.Failed);
			}
			if (RevpayOptions is null)
			{
				throw new RestException(System.Net.HttpStatusCode.PreconditionFailed, "Kindly configure the required application settings");
			}

			var payload = new AbcRequestPayload
			{
				Webguid = billPaymentCode,
				State = RevpayOptions.State,
				Hash = RevpayConfig.HashForReferenceVerification(RevpayOptions.Key, billPaymentCode, RevpayOptions.State),
				ClientId = RevpayOptions.ClientId,
				Type = RevpayOptions.Type
			};

			

			try
			{
				var httpResponse = await HttpClientExtensions.PostAsJson(_httpClient, RevpayOptions.BaseUrl,
					RevpayOptions.ReferenceVerification, payload);

				if (httpResponse.IsSuccessStatusCode)
				{
					var revPayRes = await httpResponse.ReadContentAs<BillReferenceResponseDto>();

					//current time update
					revPayRes.CurrentDate = DateTime.Now;

					var billPayerInfo = _mapper.Map<BillPayerInfo>(revPayRes);
					billPayerInfo.PhoneNumber = phone;

					if (billPayerInfo.PayerName == null && billPayerInfo.Status == null)
						throw new RestException(HttpStatusCode.NotFound, "Record not found for the bill-code provided");
                 
					// biller information response data
					billPayerInfo.AccountInfoResponseData = JsonConvert.SerializeObject(revPayRes);
					billPayerInfo.billCode = billPaymentCode;
					billPayerInfo.PhoneNumber = phone;
					

					// bill-payer information request data
					billPayerInfo.AccountInfoRequestData = JsonConvert.SerializeObject(payload);
					billPayerInfo.AccountInfoResponseData = JsonConvert.SerializeObject(revPayRes);

					var existingBillPayerInfo = await _billPayerRepo
						.Query(x => x.billCode.ToLower() == billPayerInfo.billCode.ToLower())
						.LastOrDefaultAsync();

					if(existingBillPayerInfo is null)
						await _billPayerRepo.AddAsync(billPayerInfo);
					else
					{
						_mapper.Map(billPayerInfo, existingBillPayerInfo);
					}


					await _billPayerRepo.SaveChangesAsync();

					return new SuccessResponse<BillReferenceResponseDto>
					{
						Data = revPayRes,
						Message = "Data retrieved successfully"
					};

				}
				throw new RestException(HttpStatusCode.BadRequest, "Invalid Request");
			}
			catch (Exception ex)
			{
				throw new RestException(HttpStatusCode.InternalServerError, ex.Message);
			}
		}

		public async Task<SuccessResponse<CustomBillPaymentVerificationResponse>> PaymentVerification(string billPaymentCode)
		{
			if (string.IsNullOrEmpty(billPaymentCode))
			{
				throw new RestException(HttpStatusCode.BadRequest, ResponseMessages.Failed);
			}

			if (RevpayOptions is null)
			{
				throw new RestException(System.Net.HttpStatusCode.PreconditionFailed, "Kindly configure the required application settings");
			}

			var payload = new AbcRequestPayload
			{
				Webguid = billPaymentCode,
				State = RevpayOptions.State,
				Hash = RevpayConfig.HashForPaymentValidation(RevpayOptions.Key, billPaymentCode),
				ClientId = RevpayOptions.ClientId,
				Type = RevpayOptions.Type
			};

			var httpResponse = await HttpClientExtensions.PostAsJson(_httpClient, RevpayOptions.BaseUrl, RevpayOptions.PaymentVerification, payload);

			if (httpResponse.IsSuccessStatusCode)
			{
				var revPayJsonResponse = await httpResponse.Content.ReadAsStringAsync();
				var revPayRes = JsonConvert.DeserializeObject<BillPaymentVerificationResponseDto>(revPayJsonResponse);

				var billTransaction = await _billTransactionsRepo
					.Query(x=>x.BillNumber == billPaymentCode && x.Status == ETransactionStatus.Successful.ToString())
					.OrderByDescending(x=>x.DateCompleted).FirstOrDefaultAsync();

				if (billTransaction is null)
					return new SuccessResponse<CustomBillPaymentVerificationResponse>
					{
						Data = new CustomBillPaymentVerificationResponse
						{
							Receipt = revPayRes.receipt,
							AmountPaid = revPayRes.amountpaid,
						},
						Message = "Incomplete transaction details"
					};

				var respoonse = new CustomBillPaymentVerificationResponse
				{
					Name = billTransaction.PayerName,
					PropertyId = billTransaction.Pid,
					AmountDue = billTransaction.AmountDue.ToString(),
					AmountPaid = billTransaction.AmountPaid.ToString(),
					PaymentDate = billTransaction.DateCompleted,
					Receipt = billTransaction.ReceiptUrl
				};

				return new SuccessResponse<CustomBillPaymentVerificationResponse>
				{
					Data = respoonse,
					Message = "Data retrieved successfully"
				};
			}
			throw new RestException(System.Net.HttpStatusCode.BadRequest, "Invalid Request: unable to verify the status of this transaction");
		}

		public async Task<SuccessResponse<IEnumerable<CustomBillPaymentVerificationResponse>>> GetAllTransactions()
		{
			var billTransactions = await _billTransactionsRepo.GetAllAsync();
			if (billTransactions == null)
				return new SuccessResponse<IEnumerable<CustomBillPaymentVerificationResponse>>
				{
					Data = null,
					Message = "unable to retrive transactions"
				};

			var response = _mapper.Map<IEnumerable<CustomBillPaymentVerificationResponse>>(billTransactions);
			return new SuccessResponse<IEnumerable<CustomBillPaymentVerificationResponse>>
			{
				Data = response,
				Message = "Data retrieved successfully"
            };
		}

	}
}
