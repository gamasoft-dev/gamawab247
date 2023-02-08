using Microsoft.EntityFrameworkCore;

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
    }
}
