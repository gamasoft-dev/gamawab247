using Application.AutofacDI;
using BillProcessorAPI.Dtos;
using BillProcessorAPI.Dtos.Common;
using BillProcessorAPI.Dtos.Flutterwave;
using BillProcessorAPI.Helpers;

namespace BillProcessorAPI.Services.Interfaces;

public interface IFlutterwaveMgtService: IAutoDependencyService
{
    Task<SuccessResponse<PaymentCreationResponse>> CreateTransaction(string email, decimal amount, string billPaymentCode, string phoneNumber);
    Task<bool> VerifyTransaction(string transactionReference);
    Task<SuccessResponse<PaymentConfirmationResponse>> PaymentConfirmation(string status, string txRef, string transactionId);
    Task<SuccessResponse<string>> PaymentNotification(WebHookNotificationWrapper model);
    Task<FailedWebhookResponseModel> ResendWebhook(FailedWebhookRequest model);
}