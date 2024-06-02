using System;
using System.Globalization;
using Application.AuditServices;
using Application.DTOs;
using Domain.Common.ShortLink.ValueObjects;
using Domain.Entities.Identities;
using Infrastructure;
using Infrastructure.Cache;
using Infrastructure.Data.DbContext;
using Infrastructure.Data.DbContext.DbAuditFilters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace BroadcastMessageServiceWorker.ServiceExtension
{
    public static class ServiceExtensions
    {
        private static readonly ILoggerFactory ContextLoggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });

        /// <summary>
        /// Configure binding of IConfigurations to typed object for better maintainability.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void ConfigureIOObjects(this IServiceCollection services, IConfiguration configuration)
        {
       
            services.Configure<SystemSettingsConfig>(configuration.GetSection("Config"));
            services.Configure<Dialog360Settings>(configuration.GetSection("Dialog360Setting"));
            services.Configure<CutlyOptions>(configuration.GetSection("CutlyOptions"));
            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
        }

        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(opts =>
            {
                opts.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), x =>
                {
                    x.MigrationsAssembly("Infrastructure.Data");
                    x.EnableRetryOnFailure();
                })
                .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
                 .EnableSensitiveDataLogging();
            });
        }

        public static void ConfigureRepositoryManager(this IServiceCollection services)
        {
            services.AddInfrastructureServices();
        }

        public static void ConfigureMvcAndAutomapper(this IServiceCollection services)
        {
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }

        public static void ConfigureRedisCache(this IServiceCollection services, IConfiguration configuration)
        {
            RedisCacheConfig config = new RedisCacheConfig();
            configuration.Bind(nameof(RedisCacheConfig), config);
            services.AddSingleton(config);

            services.AddDistributedRedisCache(options =>
            {
                var redisConfig = new ConfigurationOptions()
                {
                    EndPoints = { { config.Server, config.Port } },
                    AbortOnConnectFail = false,
                    ConnectTimeout = 5000,
                    ConnectRetry = 3,
                    Ssl = false,
                    SslProtocols = System.Security.Authentication.SslProtocols.Tls12,
                    Password = config.Auth
                };
                options.Configuration = redisConfig.ToString();
                options.InstanceName = config.InstanceName;
            });
            services.AddScoped<ICacheService, CacheService>();
        }

        public static void ConfigureIdentity(this IServiceCollection services)
        {
            var builder = services.AddIdentityCore<User>(opts =>
            {
                opts.Password.RequireDigit = true;
                opts.Password.RequiredLength = 8;
                opts.Password.RequireLowercase = true;
                opts.Password.RequireUppercase = true;
                opts.Password.RequireNonAlphanumeric = false;
                opts.User.RequireUniqueEmail = true;
            }).AddRoles<Role>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

            // custom auditing service at db level
            services.AddScoped<IPersistenceAudit, PersistenceAuditService>();
        }
    }
}

