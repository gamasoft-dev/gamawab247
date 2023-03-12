using System;
using System.Globalization;
using Application.AuditServices;
using Application.DTOs;
using Application.Services.Implementations;
using Application.Services.Interfaces;
using Domain.Entities.Identities;
using Infrastructure;
using Infrastructure.Cache;
using Infrastructure.Data.DbContext;
using Infrastructure.Data.DbContext.DbAuditFilters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace FormProcessingWorker
{
    public static class ServiceExtensions
    {
        private static readonly ILoggerFactory ContextLoggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });

        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(opts =>
            {
                opts.AddPolicy("CorsPolicy", builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });
        }

        public static void ConfigureGlobalization(this IServiceCollection services)
        {
            services.AddLocalization();
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("en"),
                };
                options.DefaultRequestCulture = new RequestCulture("en");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });


            //services.AddSingleton<IValidationLocalizerService, ValidationLocalizerService>();
            //services.AddSingleton<IRestErrorLocalizerService, RestErrorLocalizerService>();
        }

        public static void ConfigureIisIntegration(this IServiceCollection services)
        {
            services.Configure<IISOptions>(options => { });
        }

        /// <summary>
        /// Configure binding of IConfigurations to typed object for better maintainability.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void ConfigureIOObjects(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<EmailSettings>(configuration.GetSection(nameof(EmailSettings)));
            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            services.Configure<SystemSettingsConfig>(configuration.GetSection("Config"));
            services.Configure<Dialog360Settings>(configuration.GetSection("Dialog360Setting"));
            services.Configure<Domain.ViewModels.JwtConfigSettings>(configuration.GetSection("JwtSettings"));
            services.Configure<RedisCacheConfig>(configuration.GetSection("RedisCacheConfig"));
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
            services.AddRepositories();
        }

        
        public static void ConfigureCustomServices(this IServiceCollection services)
        {
            //services.AddSingleton<ISessionManagement, SessionManagement>();
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
            services.AddTransient<ICacheService, CacheService>();
        }

        public static void ConfigureHangfire(this IServiceCollection services, IConfiguration configuration)
        {
            //    services.AddHangfire(x =>
            //        x.UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection")));
            //    services.AddHangfireServer();
        }
       
        public static void ConfigureLoggerService(this IServiceCollection services)
        {
            services.AddScoped<ILoggerManager, LoggerManager>();
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

