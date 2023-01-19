﻿using Application.AuditServices;
using Application.DTOs;
using Application.Services.Implementations;
using Application.Services.Interfaces;
using Domain.Entities.Identities;
using Infrastructure;
using Infrastructure.Data.DbContext;
using Infrastructure.Data.DbContext.DbAuditFilters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace BusinessConfigWorker.ServiceExtension
{
    public static class ServiceExtensionCollection
    {
        private static readonly ILoggerFactory ContextLoggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });

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

        //public static void ConfigureMvcAndAutomapper(this IServiceCollection services)
        //{
        //    services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        //}

        public static void ConfigureRedisCache(this IServiceCollection services, IConfiguration configuration)
        {
            RedisCacheConfig config = new RedisCacheConfig();
            configuration.Bind(nameof(RedisCacheConfig), config);
            services.AddSingleton(config);

            services.AddDistributedRedisCache(options =>
            {
                var redisConfig = new StackExchange.Redis.ConfigurationOptions()
                {
                    EndPoints = { { config.Server, config.Port } },
                };
                options.Configuration = redisConfig.ToString();
                options.InstanceName = config.InstanceName;
            });
            services.AddTransient<ICacheService, CacheService>();
        }

        public static void ConfigureLoggerService(this IServiceCollection services)
        {
            services.AddScoped<ILoggerManager, LoggerManager>();
        }

        public static void ConfigureIdentity(this IServiceCollection services)
        {
            // custom auditing service at db level
            services.AddScoped<IPersistenceAudit, PersistenceAuditService>();
        }
    }
}
