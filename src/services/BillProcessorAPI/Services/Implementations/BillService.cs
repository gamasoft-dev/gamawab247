using AutoMapper;
using BillProcessorAPI.Dtos;
using BillProcessorAPI.Entities;
using BillProcessorAPI.Enums;
using BillProcessorAPI.Helpers;
using BillProcessorAPI.Helpers.Revpay;
using BillProcessorAPI.Repositories.Interfaces;
using BillProcessorAPI.Services.Interfaces;
using Domain.Common;
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
		private readonly IRepository<BillTransaction> _billTransactions;
		private readonly RevpayOptions RevpayOptions;
		private readonly IMapper _mapper;

        public BillService(IBillPayerRepository billPayerRepo,
            IOptions<RevpayOptions> options,
            IMapper mapper,
            IRepository<BillTransaction> billTransactions,
            HttpClient httpClient,
            IConfiguration config)
        {
            _billPayerRepo = billPayerRepo;
            RevpayOptions = options.Value;
            _mapper = mapper;

            _billTransactions = billTransactions;
            _httpClient = httpClient;
            _config = config;
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
					revPayRes.CurrentDate = DateTime.Now;

					var mappedResponse = _mapper.Map<BillPayerInfo>(revPayRes);
					mappedResponse.PhoneNumber = phone;

					if (mappedResponse.PayerName == null && mappedResponse.Status == null)
						throw new RestException(HttpStatusCode.NotFound, "Record not found for the bill-code provided");
                 
					// biller information response data
					mappedResponse.AccountInfoResponseData = JsonConvert.SerializeObject(revPayRes);
					mappedResponse.billCode = billPaymentCode;
					mappedResponse.PhoneNumber = phone;
					

					//bill-payer information request data
					mappedResponse.AccountInfoRequestData = JsonConvert.SerializeObject(payload);

					await _billPayerRepo.AddAsync(mappedResponse);
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

		public async Task<SuccessResponse<BillPaymentVerificationResponseDto>> PaymentVerification(string billPaymentCode)
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

				var billTransaction = _mapper.Map<BillTransaction>(revPayRes);

				// bill-transaction response data
				billTransaction.PaymentInfoResponseData = revPayJsonResponse;

				//bill-transaction  request data
				billTransaction.PaymentInfoRequestData = JsonConvert.SerializeObject(payload);

				await _billTransactions.SaveChangesAsync();

				return new SuccessResponse<BillPaymentVerificationResponseDto>
				{
					Data = revPayRes,
					Message = "Data retrieved successfully"
				};
			}
			throw new RestException(System.Net.HttpStatusCode.BadRequest, "Invalid Request: unable to verify the status of this transaction");
		}

	}
}
