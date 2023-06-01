using Application.AutofacDI;
using BillProcessorAPI.Dtos;
using BillProcessorAPI.Dtos.Common;
using BillProcessorAPI.Dtos.Flutterwave;
using BillProcessorAPI.Helpers;

namespace BillProcessorAPI.Services.Interfaces
{
    public interface IFlutterwaveService
    {
        Task<SuccessResponse<PaymentCreationResponse>> CreateTransaction(string email, decimal amount, string billPaymentCode);
        Task<bool> VerifyTransaction(string transactionReference);
        Task<SuccessResponse<PaymentConfirmationResponse>> PaymentConfirmation(string status, string tx_ref, string transaction_id);
        Task<SuccessResponse<string>> PaymentNotification(WebHookNotificationWrapper model);
        Task<FailedWebhookResponseModel> ResendWebhook(FailedWebhookRequest model);
    }
}
