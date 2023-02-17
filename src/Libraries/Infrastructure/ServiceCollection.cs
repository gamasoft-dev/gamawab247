using System;
using System.Net.Http;
using System.Threading.Tasks;
using Application.Services.Implementations;
using Application.Services.Interfaces;
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
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddHttpClient<IHttpService, HttpService>()
                .AddPolicyHandler(GetPollyPolicy());
        }

        /// <summary>
        /// Polly configuration for resilient http calls
        /// </summary>
        /// <returns></returns>
        private static AsyncRetryPolicy<HttpResponseMessage> GetPollyPolicy()
        {
            // Create the retry policy we want
            var retryPolicy = HttpPolicyExtensions
                            .HandleTransientHttpError() // HttpRequestException, 5XX and 408
                            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(retryAttempt));

            return retryPolicy;
        }

    }
}
