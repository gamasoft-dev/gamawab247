using BillProcessorAPI.Dtos;
using BillProcessorAPI.Helpers;
using BillProcessorAPI.Helpers.Revpay;
using BillProcessorAPI.Repositories.Interfaces;
using BillProcessorAPI.Services.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BillProcessorAPI.Services.Implementations
{
    public class RevpayService : IRevpayService
	{
		private readonly ApiClient _apiClient;
		private readonly IBillPayerRepository _billPayerRepo;
		private readonly IOptions<RevpayOptions> _options;
		public RevpayService(IBillPayerRepository billPayerRepo, IOptions<RevpayOptions> options, ApiClient apiClient)
		{
			_billPayerRepo = billPayerRepo;
			_options = options;
			_apiClient = apiClient;
		}

		public async Task<SuccessResponse<BillPayerInfoDto>> ReferenceVerification(BillReferenceRequestDto model)
		{
			var revPayBaseUrl = _options.Value.BaseUrl;
			var revReferenceLink = _options.Value.ReferenceVerification;
			var apiKey = _options.Value.ApiKey;
			var response = await _apiClient.PostAsync(revPayBaseUrl, revReferenceLink, model);

				if (response.IsSuccessStatusCode)
				{
					var revPayJsonResponse = await response.Content.ReadAsStringAsync();
					var revPayRes = JsonConvert.DeserializeObject<SuccessResponse<BillPayerInfoDto>>(revPayJsonResponse);

					return revPayRes;

				}
				return null;
		}

		public async  Task<SuccessResponse<PaymentVerificationResponseDto>> PaymentVerification( PaymentVerificationRequestDto model)
		{
			var revPayBaseUrl = _options.Value.BaseUrl;
			var revReferenceLink = _options.Value.PaymentVerification;
			var apiKey = _options.Value.ApiKey;
			var response = await _apiClient.PostAsync(revPayBaseUrl, revReferenceLink, model);

			if (response.IsSuccessStatusCode)
			{
				var revPayJsonResponse = await response.Content.ReadAsStringAsync();
				var revPayRes = JsonConvert.DeserializeObject<SuccessResponse<PaymentVerificationResponseDto>>(revPayJsonResponse);

				return revPayRes;

			}
			return null;
		}
	}
}
