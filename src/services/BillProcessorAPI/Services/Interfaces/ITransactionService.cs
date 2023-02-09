using BillProcessorAPI.Dtos;
using BillProcessorAPI.Helpers;

namespace BillProcessorAPI.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<SuccessResponse<TransactionVerificationResponseDto>> VerifyBillTransactionAsync(TransactionVerificationInputDto input);
        Task<SuccessResponse<object>> CreateUserBillTransaction(CreateUserBillTransactionInputDto input);
        SuccessResponse<ChargesResponseDto> CalculateBillChargesOnAmount(ChargesInputDto input);
        Task<SuccessResponse<ChargesResponseDto>> CreateBillCharges(CreateBillChargeInputDto input);
        Task<SuccessResponse<IEnumerable<ChargesResponseDto>>> GetBillCharges();
    }
}
