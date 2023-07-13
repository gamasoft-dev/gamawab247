using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using API.Configurations;
using API.Middlewares;
using Application.AuditServices;
using Application.DTOs;
using Application.Services.Implementations;
using Application.Services.Interfaces;
using Gamawabs247API.Validations;
using Domain.Entities.Identities;
using Domain.ViewModels;
using FluentValidation.AspNetCore;
using Infrastructure;
using Infrastructure.Cache;
using Infrastructure.Data.DbContext;
using Infrastructure.Data.DbContext.DbAuditFilters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.SwaggerGen;
using Domain.Common.ShortLink.ValueObjects;

namespace API.Extensions
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
            services.Configure<JwtConfigSettings>(configuration.GetSection("JwtSettings"));
            services.Configure<RedisCacheConfig>(configuration.GetSection("RedisCacheConfig"));
            services.Configure<CutlyOptions>(configuration.GetSection(nameof(CutlyOptions)));
        }

        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(opts =>
            {
                opts.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), x =>
                 {
                     x.MigrationsAssembly("Infrastructure.Data");
                 })
                .EnableDetailedErrors(true)
                .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
            });
        }

        public static void ConfigureRepositoryManager(this IServiceCollection services)
        {
            services.AddInfrastructureServices();
        }

        public static void ConfigureHttpClient(this IServiceCollection services)
        {
            services.AddHttpClientInfrastructure();
        }

        public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var jwtUserSecret = jwtSettings.GetSection("Secret").Value;

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.GetSection("ValidIssuer").Value,
                    ValidAudience = jwtSettings.GetSection("ValidAudience").Value,
                    IssuerSigningKey = new
                        SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtUserSecret))
                };
            });
        }

        public static void ConfigureCustomServices(this IServiceCollection services)
        {
           //services.AddSingleton<ISessionManagement, SessionManagement>();
        }
        
        public static void ConfigureMvc(this IServiceCollection services)
        {
            services?.AddMvc()?.SetCompatibilityVersion(CompatibilityVersion.Latest)
                .ConfigureApiBehaviorOptions(o =>
                {
                    o.InvalidModelStateResponseFactory = context => new ValidationFailedResult(context.ModelState);
                }).AddFluentValidation(x =>
                    x.RegisterValidatorsFromAssemblyContaining<UserValidator>());

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
                    ConnectTimeout = 100000,
                    ConnectRetry = 10,
                    Ssl = false,
                    Password = config.Auth
                };
                options.Configuration = redisConfig.ToString();
                options.InstanceName = config.InstanceName;
            });
            services.AddSingleton<ICacheService, CacheService>();
        }

        public static void ConfigureHangfire(this IServiceCollection services, IConfiguration configuration)
        {
        //    services.AddHangfire(x =>
        //        x.UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection")));
        //    services.AddHangfireServer();
        }

        public static void ConfigureApiVersioning(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApiVersioning(opt =>
            {
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.DefaultApiVersion = new ApiVersion(1, 0);
                opt.ReportApiVersions = true;
            });
            services.AddVersionedApiExplorer(opt =>
            {
                opt.GroupNameFormat = "'v'VVV";
                opt.SubstituteApiVersionInUrl = true;
            });
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddMvcCore().AddApiExplorer();
        }

        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.OperationFilter<RemoveVersionFromParameter>();
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}
                    }
                });

                // TODO: Fix the Docker error on this
                // var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                // var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                // c.IncludeXmlComments(xmlPath);
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
                    new CultureInfo("fr"),
                };
                options.DefaultRequestCulture = new RequestCulture("en");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });
            //services.AddSingleton<IValidationLocalizerService, ValidationLocalizerService>();
            //services.AddSingleton<IRestErrorLocalizerService, RestErrorLocalizerService>();
        }

        public static void ConfigureLoggerService(this IServiceCollection services)
        {
            services.AddScoped<ILoggerManager, LoggerManager>();
        }

        public static void ConfigureIdentity(this IServiceCollection services)
        {
            var builder = services.AddIdentityCore<User>(opts =>
            {
                opts.Password.RequireDigit = false;
                opts.Password.RequiredLength = 8;
                opts.Password.RequireLowercase = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequireNonAlphanumeric = false;
                opts.User.RequireUniqueEmail = true;
            }).AddRoles<Role>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

            // custom auditing service at db level
            services.AddScoped<IPersistenceAudit, PersistenceAuditService>();
        }
    }
}
