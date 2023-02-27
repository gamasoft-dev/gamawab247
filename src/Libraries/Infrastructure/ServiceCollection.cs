using System;
using System.Net.Http;
using System.Threading.Tasks;
using Infrastructure.Http;
using Infrastructure.Repositories.Implementations;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;

namespace Infrastructure
{
    public static class ServiceCollection
    {
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserActivityRepository, UserActivityRepository>();
            services.AddScoped<IMessageLogRepository, MessageLogRepository>();
            services.AddScoped<IWhatsappUserRepository, WhatsappUserRepository>();

        }

        public static void AddHttpClientInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IHttpService, HttpService>();

            services.AddHttpClient("GamaWabsAPI")
                    .AddPolicyHandler(GetPollyPolicy("GamaWabsAPI"));

        }

        /// <summary>
        /// Polly configuration for resilient http calls
        /// </summary>
        /// <returns></returns>
        private static AsyncRetryPolicy<HttpResponseMessage> GetPollyPolicy(string name)
        {
            // Create the retry policy we want
            return HttpPolicyExtensions
                            .HandleTransientHttpError() // HttpRequestException, 5XX and 408
                            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(retryAttempt),
                            onRetryAsync: (dr, ts) =>
                            {
                                Console.WriteLine($"Retrying call to api for service name {name}");
                                return Task.CompletedTask;
                            } );

        }
    }
}
