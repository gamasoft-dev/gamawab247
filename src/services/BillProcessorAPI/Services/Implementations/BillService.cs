using AutoMapper;
using BillProcessorAPI.Dtos;
using BillProcessorAPI.Entities;
using BillProcessorAPI.Enums;
using BillProcessorAPI.Helpers;
using BillProcessorAPI.Helpers.Revpay;
using BillProcessorAPI.Repositories.Interfaces;
using BillProcessorAPI.Services.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;

namespace BillProcessorAPI.Services.Implementations
{
	public class BillService : IBillService
	{
		private readonly IConfiguration _config;
		private readonly HttpClient _httpClient;
		private readonly IBillPayerRepository _billPayerRepo;
		private readonly IRepository<BillTransaction> _billTransactions;
		private RevpayOptions RevpayOptions { get; }
		private readonly IMapper _mapper;

        public BillService(IBillPayerRepository billPayerRepo, IOptions<RevpayOptions> options, IMapper mapper, IRepository<BillTransaction> billTransactions, HttpClient httpClient, IConfiguration config)
        {
            _billPayerRepo = billPayerRepo;
			RevpayOptions = options.Value;
            _mapper = mapper;

            _billTransactions = billTransactions;
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<SuccessResponse<BillPayerInfoDto>> ReferenceVerification(BillReferenceRequestDto model)
		{
			BillPayerInfoDto dto = MockResponseBillPayerInfo();
			if (_config["IsTest"].Equals("true", StringComparison.OrdinalIgnoreCase))
				return new SuccessResponse<BillPayerInfoDto>
				{
					Data = dto,
					Message = "Data retrieved successfully"
				};

			if (RevpayOptions is null)
			{
				throw new RestException(System.Net.HttpStatusCode.PreconditionFailed, "Kindly configure the required application settings");
			}

			//var apiKey = _options.Value.ApiKey;
			try
			{
				_httpClient.BaseAddress = new Uri(RevpayOptions.BaseUrl);
				var httpResponse = await _httpClient.PostAsJson(RevpayOptions.ReferenceVerification, model, _config, new Dictionary<string, string>
				{
					{ "webguid", RevpayOptions.WebGuid },
					{ "state", RevpayOptions.State },
					{ "hash", RevpayOptions.Key },
					{ "ClientId", RevpayOptions.ClientId }
				});

				if (httpResponse.IsSuccessStatusCode)
				{
					var revPayRes = await httpResponse.ReadContentAs<BillPayerInfoDto>();

					var mappedResponse = _mapper.Map<BillPayerInfo>(revPayRes);
					// biller information response data
					mappedResponse.AccountInfoResponseData = JsonConvert.SerializeObject(revPayRes);
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
			BillPaymentVerificationResponseDto dto = MockResponsePaymentVerification();
			if (_config["IsTest"].Equals("true", StringComparison.OrdinalIgnoreCase))
				return new SuccessResponse<BillPaymentVerificationResponseDto>
				{
					Data = dto,
					Message = "Data retrieved successfully"
				};

			_httpClient.BaseAddress = new Uri(RevpayOptions.BaseUrl);
			var response = await _httpClient.PostAsJson(RevpayOptions.PaymentVerification, model, _config, new Dictionary<string, string>
				{
					{ "webguid", RevpayOptions.WebGuid },
					{ "state", RevpayOptions.State },
					{ "hash", RevpayOptions.Key },
					{ "ClientId", RevpayOptions.ClientId }
				});

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

		private static BillPaymentVerificationResponseDto MockResponsePaymentVerification()
		{
			return new()
			{
				Receipt = "Lasg 42014141/RPXIEASA = 15000.00 paid on Apr 6 2021 12:00AM By ADEYINKA OLUMIDE OLUGBILE <<N-1549181>> for 4040098/Administrative Charges",
				Status = "SUCCESS",
				StatusMessage = "Valid Receipt"
			};
		}

        private static BillPayerInfoDto MockResponseBillPayerInfo()
        {
            return new()
            {
                AmountDue = "20000",
                Status = "SUCCESS",
                CreditAccount = "0002153323",
                PayerName = "OLUFEMI JUDGE DOMINGO",
                AgencyCode = "4310000",
                RevenueCode = "0008372",
                OraAgencyRev = "11004-33601",
                State = "XXSG",
                StatusMessage = "OK",
                Pid = "N-9546578",
                Currency = "NGN",
                AcctCloseDate = "9/1/2020 12:00:00 AM",
                ReadOnly = "No",
                MinAmount = "0.0000",
                MaxAmount = "0.0000"
            };
        }
    }
}
