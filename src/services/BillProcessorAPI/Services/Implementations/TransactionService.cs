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
        private readonly IBillTransactionRepository _billTransactionRepository;
        private readonly IRepository<BillPayerInfo> _billPayerRepository;
        private readonly IRepository<BillCharge> _billChargeRepository;
        private readonly ILoggerManager _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;
        private BillTransactionSettings BillTransactionSettings { get; }
        public TransactionService(
            IBillTransactionRepository billTransactionRepository,
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
            var billTransaction = await _billTransactionRepository.FirstOrDefault(x => x.SystemReference == input.TransactionReference);
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
                billTransaction.Status = ETransactionStatus.Failed;
                _logger.LogInfo($"{nameof(VerifyBillTransactionAsync)} RESPONSE => {httpResponse.ReasonPhrase} {httpResponse.RequestMessage}");
                throw new RestException(HttpStatusCode.BadRequest, httpResponse.ReasonPhrase);
            }

            var result = await httpResponse.ReadContentAs<TransactionVerificationResponseDto>();

            _logger.LogInfo($"{nameof(VerifyBillTransactionAsync)} RESPONSE => {JsonConvert.SerializeObject(result)}");

            if (result.Data is null)
            {
                billTransaction.Status = ETransactionStatus.Failed;
                _logger.LogInfo($"{nameof(VerifyBillTransactionAsync)} RESPONSE => {JsonConvert.SerializeObject(result)}");
                throw new RestException(HttpStatusCode.BadRequest, result.Message);
            }

            if (!result.Data.Status.Equals("successful", StringComparison.OrdinalIgnoreCase) ||
                !result.Data.Amount.Equals(input.AmountPaid) ||
                !result.Data.TxRef.Equals(input.TransactionReference))
            {
                billTransaction.Status = ETransactionStatus.Failed;
                _logger.LogInfo($"{nameof(VerifyBillTransactionAsync)} RESPONSE => {JsonConvert.SerializeObject(result)}");
                throw new RestException(HttpStatusCode.BadRequest, result.Message);
            }            

            billTransaction.Status = ETransactionStatus.Successfull;
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
            billTransaction.Status = ETransactionStatus.Pending;

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

        public SuccessResponse<ChargesResponseDto> CalculateBillChargesOnAmount(ChargesInputDto input)
        {    
            var charge = GetAmountCharge(input);

            var result = _mapper.Map<ChargesResponseDto>(input);

            result.AmountCharge= charge;

            return new SuccessResponse<ChargesResponseDto>
            {
                Message = "Charges calculated successfully",
                Data = result
            };
        }

        public async Task<SuccessResponse<ChargesResponseDto>> CreateBillCharges(CreateBillChargeInputDto input)
        {
            var charge = _mapper.Map<BillCharge>(input);

            await _billChargeRepository.AddAsync(charge);
            await _billChargeRepository.SaveChangesAsync();

            var result = _mapper.Map<ChargesResponseDto>(charge);

            return new SuccessResponse<ChargesResponseDto>
            {
                Message = "Bill charge created successfully",
                Data = result
            };
        }

        public async Task<SuccessResponse<IEnumerable<ChargesResponseDto>>> GetBillCharges()
        {
            var billCharges = await _billChargeRepository.GetAllAsync();

            var result = _mapper.Map<IEnumerable<ChargesResponseDto>>(billCharges);

            return new SuccessResponse<IEnumerable<ChargesResponseDto>>
            {
                Message = "Bill charges retrieved successfully",
                Data = result
            };
        }

        private static int GetAmountCharge(ChargesInputDto input)
        {
            var charge = (Math.Round((decimal)input.PercentageCharge, 2) / 100) * input.Amount;
            var chargeAmount = Math.Round(charge, 0);

            if (chargeAmount <= input.MinChargeAmount)
                return (int)input.MinChargeAmount;
            
            if (chargeAmount > input.MinChargeAmount && chargeAmount < input.MaxChargeAmount)
                return (int)chargeAmount;
            
            if (chargeAmount >= input.MaxChargeAmount)
                return (int)input.MaxChargeAmount;

            return default;
        }
    }
}
