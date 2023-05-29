using Application.AutofacDI;
using BillProcessorAPI.Dtos;
using BillProcessorAPI.Helpers;

namespace BillProcessorAPI.Services.Interfaces
{
    // useful for all configurational settings to the db
    public interface IConfigurationService
    {
        Task<SuccessResponse<IEnumerable<ChargesResponseDto>>> GetBillChargesByBusiness(Guid businessId);
        SuccessResponse<ChargesResponseDto> CalculateBillChargesOnAmount(ChargesInputDto input);
        Task<SuccessResponse<ChargesResponseDto>> CreateBillCharges(CreateBillChargeInputDto input);
        Task<SuccessResponse<IEnumerable<ChargesResponseDto>>> GetBillCharges();
    }
}
