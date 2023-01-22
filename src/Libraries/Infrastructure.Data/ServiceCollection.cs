using Infrastructure.Data.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data
{
    public static class ServiceCollection
    {
        public static readonly ILoggerFactory contextLoggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });

        // by default, the service containing db-context is preset as default service to handle migrations...
        public static IServiceCollection AddClientDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DB_CONNECTION_STRING");
            services.AddDbContext<AppDbContext>((option) =>
            {
                option.UseSqlServer(connectionString)
                    .UseLoggerFactory(contextLoggerFactory); 
            });
            return services;
        }
    }
}
