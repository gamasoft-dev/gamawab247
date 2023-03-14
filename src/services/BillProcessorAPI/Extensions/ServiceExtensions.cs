using BillProcessorAPI.Middlewares;
using BillProcessorAPI.Services.Implementations;
using BillProcessorAPI.Services.Interfaces;
using BillProcessorAPI.Validators;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
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
            serviceCollection.AddHttpClient<IBillService, BillService>()
                .AddPolicyHandler(retryPolicy);
        }

        public static void ConfigureMvc(this IServiceCollection services)
        {
            services?.AddMvc()?.SetCompatibilityVersion(CompatibilityVersion.Latest)
                .ConfigureApiBehaviorOptions(o =>
                {
                    o.InvalidModelStateResponseFactory = context => new ValidationFailedResult(context.ModelState);
                }).AddFluentValidation(x =>
                    x.RegisterValidatorsFromAssemblyContaining<TransactionValidator>());

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }
    }
}
