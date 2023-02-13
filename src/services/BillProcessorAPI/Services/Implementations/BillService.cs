using AutoMapper;
using BillProcessorAPI.Dtos;
using BillProcessorAPI.Entities;
using BillProcessorAPI.Enums;
using BillProcessorAPI.Helpers;
using BillProcessorAPI.Helpers.Revpay;
using BillProcessorAPI.Repositories.Interfaces;
using BillProcessorAPI.Services.Interfaces;
using Domain.Entities.Identities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Linq.Expressions;
using System.Net;

namespace BillProcessorAPI.Services.Implementations
{
    public class BillService : IBillService
	{
		private readonly ApiClient _apiClient;
		private readonly IBillPayerRepository _billPayerRepo;
		private readonly IRepository<BillTransaction> _billTransactions;
		private readonly IOptions<RevpayOptions> _options;
		private readonly IMapper _mapper;
	
		public BillService(IBillPayerRepository billPayerRepo, IOptions<RevpayOptions> options, ApiClient apiClient, IMapper mapper, IRepository<BillTransaction> billTransactions)
		{
			_billPayerRepo = billPayerRepo;
			_options = options;
			_apiClient = apiClient;
			_mapper = mapper;
			
			_billTransactions = billTransactions;
		}

		public async Task<SuccessResponse<BillPayerInfoDto>> ReferenceVerification(BillReferenceRequestDto model)
		{
			if (_options is null)
			{
				throw new RestException(System.Net.HttpStatusCode.PreconditionFailed, "Kindly configure the required application settings");
			}

			//var apiKey = _options.Value.ApiKey;
			try
			{
				var response = await _apiClient.PostAsync(_options.Value.BaseUrl, _options.Value.ReferenceVerification, model);

				if (response.IsSuccessStatusCode)
				{
					var revPayJsonResponse = await response.Content.ReadAsStringAsync();
					var revPayRes = JsonConvert.DeserializeObject<BillPayerInfoDto>(revPayJsonResponse);

					var mappedResponse = _mapper.Map<BillPayerInfo>(revPayRes);
					// biller information response data
					mappedResponse.AccountInfoResponseData = revPayJsonResponse;
					//bill-payer information request data
					mappedResponse.AccountInfoRequestData = JsonConvert.SerializeObject(model);

					await _billPayerRepo.AddAsync(mappedResponse);
					await _billPayerRepo.SaveChangesAsync();

					return new SuccessResponse<BillPayerInfoDto>
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

		public async  Task<SuccessResponse<BillPaymentVerificationResponseDto>> PaymentVerification( BillPaymentVerificationRequestDto model)
		{
			var revPayBaseUrl = _options.Value.BaseUrl;
			var revReferenceLink = _options.Value.PaymentVerification;
			//var apiKey = _options.Value.ApiKey;
			var response = await _apiClient.PostAsync(revPayBaseUrl, revReferenceLink, model);

			if (response.IsSuccessStatusCode)
			{
				var revPayJsonResponse = await response.Content.ReadAsStringAsync();
				var revPayRes = JsonConvert.DeserializeObject<BillPaymentVerificationResponseDto>(revPayJsonResponse);

				var billTransaction = _mapper.Map<BillTransaction>(revPayRes);
				// bill-transaction response data
				billTransaction.PaymentInfoResponseData = revPayJsonResponse;
				//bill-transaction  request data
				billTransaction.PaymentInfoRequestData = JsonConvert.SerializeObject(model);
				
				if (revPayRes.Status == EAbcTransactionStatus.SUCCESS.ToString())
				{
					billTransaction.Status = ETransactionStatus.Successful.ToString();
				}
				
				await _billTransactions.SaveChangesAsync();

				return new SuccessResponse<BillPaymentVerificationResponseDto>
				{
					Data = revPayRes,
					Message = "Data retrieved successfully"
				};
			}
			throw new RestException(System.Net.HttpStatusCode.BadRequest, "Invalid Request, unable to verify the status of this transaction");
		}
	}
}
