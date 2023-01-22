using Infrastructure.Repositories.Implementations;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class ServiceCollection
    {
        public static void AddRepositories(this IServiceCollection services)
        {
            //services.AddClientDbContext(configuration);
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient<IUserActivityRepository, UserActivityRepository>();
            //services.AddTransient<IBusinessMessageSettingsRepository, BusinessMessagingSettingsRepository>();
            //services.AddTransient<IBusinessRepository, BusinessRepository>();
            services.AddScoped<IWhatsappUserRepository, WhatsappUserRepository>();
            services.AddScoped<IMessageLogRepository, MessageLogRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            //services.AddTransient<IUserRepository, UserRepository>();
        }
    }
}
