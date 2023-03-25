using BillProcessorAPI.Dtos;
using BillProcessorAPI.Dtos.Paythru;
using BillProcessorAPI.Helpers;

namespace BillProcessorAPI.Services.Interfaces
{
    public interface IPayThruService
    {
        Task<SuccessResponse<PaythruPaymentResponseDto>> CreatePayment(int amount, string billCode);
        Task<SuccessResponse<PaymentVerificationResponseDto>> VerifyPayment(NotificationRequestWrapper Url);
        Task<SuccessResponse<bool>> ConfirmPayment(ConfirmPaymentRequest model);
    }
}
