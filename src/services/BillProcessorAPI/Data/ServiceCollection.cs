using BillProcessorAPI.Helpers.Revpay;
using BillProcessorAPI.Repositories.Implementations;
using BillProcessorAPI.Repositories.Interfaces;
using BillProcessorAPI.Services.Implementations;
using BillProcessorAPI.Services.Interfaces;
using Infrastructure.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace BillProcessorAPI.Data
{
    public static class ServiceCollection
    {
        public static readonly ILoggerFactory contextLoggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });

        // by default, the service containing db-context is preset as default service to handle migrations...
        public static void AddClientDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<BillProcessorDbContext>((option) =>
            {
                option.UseNpgsql(connectionString)
                    .UseLoggerFactory(contextLoggerFactory);
            });
        }


        public static void ConfigService(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<IBillService, BillService>();
            services.AddScoped<ILoggerManager, LoggerManager>();
            services.AddScoped<IPayThruService, PayThruService>();
            services.AddScoped<IConfigurationService, ConfigurationService>();
            services.AddScoped<IFlutterwaveService, FlutterwaveService>();

			var revpaySection = config.GetSection("RevpayConfig");
			services.Configure<RevpayOptions>(revpaySection);
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IBillPayerRepository, BillPayerRepository>();
			services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<IHttpService, HttpService>();
            
        }
    }
}
