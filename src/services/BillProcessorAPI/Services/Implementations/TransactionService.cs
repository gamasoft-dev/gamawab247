using AutoMapper;
using BillProcessorAPI.Dtos;
using BillProcessorAPI.Entities;
using BillProcessorAPI.Enums;
using BillProcessorAPI.Helpers;
using BillProcessorAPI.Repositories.Interfaces;
using BillProcessorAPI.Services.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;

namespace BillProcessorAPI.Services.Implementations
{
    public class TransactionService : ITransactionService
    {
        private readonly IRepository<BillTransaction> _billTransactionRepository;
        private readonly IRepository<BillPayerInfo> _billPayerRepository;
        private readonly IRepository<BillCharge> _billChargeRepository;
        private readonly ILoggerManager _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;
        private BillTransactionSettings BillTransactionSettings { get; }
        public TransactionService(
            IRepository<BillTransaction> billTransactionRepository,
            ILoggerManager logger,
            IOptions<BillTransactionSettings> settings,
            IConfiguration configuration,
            HttpClient httpClient,
            IMapper mapper,
            IRepository<BillPayerInfo> billPayerRepository,
            IRepository<BillCharge> billChargeRepository)
        {
            _billTransactionRepository = billTransactionRepository;
            _logger = logger;
            _configuration = configuration;
            BillTransactionSettings = settings.Value;
            _httpClient = httpClient;
            _mapper = mapper;
            _billPayerRepository = billPayerRepository;
            _billChargeRepository = billChargeRepository;
        }

        public async Task<SuccessResponse<TransactionVerificationResponseDto>> VerifyBillTransactionAsync(TransactionVerificationInputDto input)
        {
            var billTransaction = await _billTransactionRepository.FirstOrDefault(x => x.TransactionReference == input.TransactionReference);
            if (billTransaction is null)
            {
                _logger.LogInfo($"{nameof(VerifyBillTransactionAsync)} REQUEST => {JsonConvert.SerializeObject(input)}");
                throw new RestException(HttpStatusCode.NotFound, "Transaction Reference not found");
            }

            _httpClient.BaseAddress = new Uri(BillTransactionSettings.FlutterwaveBaseUrl);
            var httpResponse = await _httpClient.GetAsJson($"v3/transactions/{input.TransactionId}/verify", _configuration);

            _logger.LogInfo($"{nameof(VerifyBillTransactionAsync)} REQUEST => {JsonConvert.SerializeObject(input)}");

            if (!httpResponse.IsSuccessStatusCode)
            {
                billTransaction.Status = ETransactionStatus.Failed.ToString();
                _logger.LogInfo($"{nameof(VerifyBillTransactionAsync)} RESPONSE => {httpResponse.ReasonPhrase} {httpResponse.RequestMessage}");
                throw new RestException(HttpStatusCode.BadRequest, httpResponse.ReasonPhrase);
            }

            var result = await httpResponse.ReadContentAs<TransactionVerificationResponseDto>();

            _logger.LogInfo($"{nameof(VerifyBillTransactionAsync)} RESPONSE => {JsonConvert.SerializeObject(result)}");

            if (result.Data is null)
            {
                billTransaction.Status = ETransactionStatus.Failed.ToString();
                _logger.LogInfo($"{nameof(VerifyBillTransactionAsync)} RESPONSE => {JsonConvert.SerializeObject(result)}");
                throw new RestException(HttpStatusCode.BadRequest, result.Message);
            }

            if (!result.Data.Status.Equals("successful", StringComparison.OrdinalIgnoreCase) ||
                !result.Data.Amount.Equals(input.AmountPaid) ||
                !result.Data.TxRef.Equals(input.TransactionReference))
            {
                billTransaction.Status = ETransactionStatus.Failed.ToString();
                _logger.LogInfo($"{nameof(VerifyBillTransactionAsync)} RESPONSE => {JsonConvert.SerializeObject(result)}");
                throw new RestException(HttpStatusCode.BadRequest, result.Message);
            }            

            billTransaction.Status = ETransactionStatus.Successful.ToString();
            billTransaction.GatewayTransactionCharge = result.Data.ChargedAmount;
            billTransaction.Narration = result.Data.Narration;

            _logger.LogInfo($"{nameof(VerifyBillTransactionAsync)} RESPONSE => {JsonConvert.SerializeObject(result)}");

            _billTransactionRepository.Update(billTransaction);
            await _billTransactionRepository.SaveChangesAsync();

            return new SuccessResponse<TransactionVerificationResponseDto>
            {
                Message = "Transaction verified successfully",
                Data = result
            };
        }

        public async Task<SuccessResponse<object>> CreateUserBillTransaction(CreateUserBillTransactionInputDto input)
        {
            var billPayerInfo = _mapper.Map<BillPayerInfo>(input);
            var billTransaction = _mapper.Map<BillTransaction>(input);

            await _billPayerRepository.AddAsync(billPayerInfo);

            billTransaction.UserId = billPayerInfo.Id;
            billTransaction.Status = ETransactionStatus.Pending.ToString();

            await _billTransactionRepository.AddAsync(billTransaction);
            var result = await _billTransactionRepository.SaveChangesAsync();

            if (!result)
            {
                return new SuccessResponse<object>
                {
                    Success = false,
                    Message = "Unable to create record successfully",
                    Data = null
                };
            }

            return new SuccessResponse<object>
            {
                Success = true,
                Message = "Record created successfully",
                Data = null
            };
        }
    }
}
