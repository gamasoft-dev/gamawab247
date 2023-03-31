using BillProcessorAPI.Dtos;
using BillProcessorAPI.Dtos.Common;
using BillProcessorAPI.Dtos.Paythru;
using BillProcessorAPI.Helpers;

namespace BillProcessorAPI.Services.Interfaces
{
    public interface IPayThruService
    {
        Task<SuccessResponse<PaymentCreationResponse>> CreatePayment(int amount, string billCode);
        Task<SuccessResponse<PaymentVerificationResponseDto>> VerifyPayment(NotificationRequestWrapper Url);
        Task<SuccessResponse<PaymentConfirmationResponse>> ConfirmPayment(ConfirmPaymentRequest model);
    }
}
