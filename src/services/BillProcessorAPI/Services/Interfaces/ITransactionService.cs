using Application.AutofacDI;
using BillProcessorAPI.Dtos;
using BillProcessorAPI.Helpers;

namespace BillProcessorAPI.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<SuccessResponse<TransactionVerificationResponseDto>> VerifyBillTransactionAsync(TransactionVerificationInputDto input);
        Task<SuccessResponse<object>> CreateUserBillTransaction(CreateUserBillTransactionInputDto input);
    }
}
