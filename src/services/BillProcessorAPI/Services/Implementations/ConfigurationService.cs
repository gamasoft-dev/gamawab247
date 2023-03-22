using AutoMapper;
using BillProcessorAPI.Dtos;
using BillProcessorAPI.Entities;
using BillProcessorAPI.Helpers;
using BillProcessorAPI.Helpers.Revpay;
using BillProcessorAPI.Repositories.Interfaces;
using BillProcessorAPI.Services.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net;

namespace BillProcessorAPI.Services.Implementations
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IRepository<BillCharge> _billChargeRepository;
        private readonly IRepository<Business> _businessRepository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private RevpayOptions _options;

        public ConfigurationService(IRepository<BillCharge> billChargeRepository,
            ILoggerManager logger,
            IMapper mapper,
            IRepository<Business> businessRepository,
            IOptions<RevpayOptions> options)
        {
            _billChargeRepository = billChargeRepository;
            _logger = logger;
            _mapper = mapper;
            _businessRepository = businessRepository;
            _options = options.Value;
        }

        public SuccessResponse<ChargesResponseDto> CalculateBillChargesOnAmount(LucChargesInputDto input)
        {
            var charge = CalculateAmountCharge(input, _options);

            var result = _mapper.Map<ChargesResponseDto>(input);

            result.AmountCharge = charge;
            

            return new SuccessResponse<ChargesResponseDto>
            {
                Message = "Charges calculated successfully",
                Data = result
            };
        }

        public async Task<SuccessResponse<ChargesResponseDto>> CreateBillCharges(CreateBillChargeInputDto input)
        {
            await CheckIfBusinessExist(input.BusinessId);

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

        private async Task CheckIfBusinessExist(Guid? businessId)
        {
            if (businessId is not null)
            {
                var business = await _businessRepository.FirstOrDefaultNoTracking(x => x.Id == businessId);
                if (business is null)
                    throw new RestException(HttpStatusCode.NotFound, "Business is not found");
            }
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

        private static int CalculateAmountCharge(ChargesInputDto input)
        {
            var charge = (Math.Round((decimal)options.Percentage, 2) / 100) * input.Amount;
            var chargeAmount = Math.Round(charge, 2);

            if (chargeAmount <= options.MinCharge)
                return options.MinCharge;

            if (chargeAmount > options.MinCharge && chargeAmount < options.MaximumCharge)
                return chargeAmount;

            if (chargeAmount >= options.MaximumCharge)
                return options.MaximumCharge;

            return default;
        }

        public async Task<SuccessResponse<IEnumerable<ChargesResponseDto>>> GetBillChargesByBusiness(Guid businessId)
        {
            var billCharges = await _billChargeRepository.Query(x => x.BusinessId == businessId).ToListAsync();

            var result = _mapper.Map<IEnumerable<ChargesResponseDto>>(billCharges);

            return new SuccessResponse<IEnumerable<ChargesResponseDto>>
            {
                Message = "Bill charges retrieved successfully",
                Data = result
            };
        }
    }
}
