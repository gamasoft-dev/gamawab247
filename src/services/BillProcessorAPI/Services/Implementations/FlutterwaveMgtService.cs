using BillProcessorAPI.Dtos;
using BillProcessorAPI.Dtos.Common;
using BillProcessorAPI.Dtos.Flutterwave;
using BillProcessorAPI.Helpers;
using BillProcessorAPI.Services.Interfaces;
using static BillProcessorAPI.Services.Implementations.FlutterwaveService;

namespace BillProcessorAPI.Services.Implementations;

public class FlutterwaveMgtService: IFlutterwaveMgtService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    public FlutterwaveMgtService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }
    public async Task<SuccessResponse<PaymentCreationResponse>> CreateTransaction(string email, decimal amount, string billPaymentCode)
    {
        using IServiceScope scope = _serviceScopeFactory.CreateAsyncScope();
        var flutterwaveService = scope.ServiceProvider.GetRequiredService<IFlutterwaveService>();
        return await flutterwaveService.CreateTransaction(email, amount, billPaymentCode);

    }

    public async Task<SimpleTransactionVerificationResponse> VerifyTransaction(string transactionReference)
    {
        using IServiceScope scope = _serviceScopeFactory.CreateAsyncScope();
        var flutterwaveService = scope.ServiceProvider.GetRequiredService<IFlutterwaveService>();
        
        return await flutterwaveService.VerifyTransaction(transactionReference);
    }

    public async Task<SuccessResponse<PaymentConfirmationResponse>> PaymentConfirmation(string status, string txRef, string transactionId)
    {
        using IServiceScope scope = _serviceScopeFactory.CreateAsyncScope();
        var flutterwaveService = scope.ServiceProvider.GetRequiredService<IFlutterwaveService>();
        
        return await flutterwaveService.PaymentConfirmation(status, txRef, transactionId);
    }

    public async Task<SuccessResponse<string>> PaymentNotification(WebHookNotificationWrapper model)
    {
        using IServiceScope scope = _serviceScopeFactory.CreateAsyncScope();
        var flutterwaveService = scope.ServiceProvider.GetRequiredService<IFlutterwaveService>();

        return await flutterwaveService.PaymentNotification(model);
    }

    public async Task<FailedWebhookResponseModel> ResendWebhook(FailedWebhookRequest model)
    {
        using IServiceScope scope = _serviceScopeFactory.CreateAsyncScope();
        var flutterwaveService = scope.ServiceProvider.GetRequiredService<IFlutterwaveService>();
        
        return await flutterwaveService.ResendWebhook(model);
    }
}