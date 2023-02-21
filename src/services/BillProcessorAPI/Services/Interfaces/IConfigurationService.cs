using BillProcessorAPI.Dtos;
using BillProcessorAPI.Helpers;

namespace BillProcessorAPI.Services.Interfaces
{
    public interface IConfigurationService
    {
        Task<SuccessResponse<IEnumerable<ChargesResponseDto>>> GetBillChargesByBusiness(Guid businessId);
        SuccessResponse<ChargesResponseDto> CalculateBillChargesOnAmount(ChargesInputDto input);
        Task<SuccessResponse<ChargesResponseDto>> CreateBillCharges(CreateBillChargeInputDto input);
        Task<SuccessResponse<IEnumerable<ChargesResponseDto>>> GetBillCharges();
    }
}
