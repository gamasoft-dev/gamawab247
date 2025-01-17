﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Infrastructure.Data.DbContext;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Application.Helpers;
using FormProcessingWorker.Middleware;
using ApiCustomization;
using Infrastructure;
using System.Text.Json;

namespace FormProcessingWorker
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            var builder = new ConfigurationBuilder()
                .SetBasePath(hostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("customizations.Config.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{hostingEnvironment.EnvironmentName}.json", reloadOnChange: true,
                    optional: true)
                //.AddUserSecrets(Assembly.GetAssembly(typeof(Startup)))
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureIisIntegration();
            services.ConfigureLoggerService();
            services.ConfigureSqlContext(Configuration);
            services.ConfigureRedisCache(Configuration);
            services.AddAuthentication();
            services.AddHttpContextAccessor();
            services.ConfigureRepositoryManager();
            services.ConfigureIOObjects(Configuration);
            services.AddControllers().AddJsonOptions(options => {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            }).AddXmlDataContractSerializerFormatters();
            services.ConfigureMvcAndAutomapper();
            services.ConfigureGlobalization();
            services.ConfigureApiCustomizationService(Configuration);
            services.AddInfrastructureServices();
            services.AddHttpClientInfrastructure();
            services.ConfigureIdentity();

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            services.AddHostedService<Worker>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AppDbContext dbContext)
        {
            app.UseDeveloperExceptionPage();


            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseErrorHandler();

            WebHelper.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());
        }
    }
}

