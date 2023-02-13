using BillProcessorAPI.Services.Implementations;
using BillProcessorAPI.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace BillProcessorAPI.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureHttpPollyExtension(this IServiceCollection serviceCollection)
        {
            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(retryAttempt));

            serviceCollection.AddHttpClient<ITransactionService, TransactionService>()
                .AddPolicyHandler(retryPolicy);
        }
    }
}
