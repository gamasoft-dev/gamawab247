using API.Extensions;
using API.Middlewares;
using Application.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Infrastructure.Data.DbContext;
using Microsoft.EntityFrameworkCore;
using ApiCustomization;
using Serilog;

namespace API
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
                .AddEnvironmentVariables();
            
            Configuration = builder.Build();
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureCors();
            services.ConfigureIisIntegration();
            services.ConfigureLoggerService();
            services.ConfigureIdentity();
            services.ConfigureSqlContext(Configuration);
            services.ConfigureHangfire(Configuration);
            services.ConfigureRedisCache(Configuration);
            services.AddAuthentication();
            services.ConfigureJwt(Configuration);
            services.AddHttpContextAccessor();
            services.ConfigureRepositoryManager();
            services.ConfigureHttpClient();
            services.ConfigureIOObjects(Configuration);
            services.ConfigureApiCustomizationService(Configuration);
            services.AddControllers()
                .AddXmlDataContractSerializerFormatters();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Error()
                .WriteTo.File("./LogFiles/GamawabsApiLogs.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            services.ConfigureSwagger();
            services.ConfigureApiVersioning(Configuration);
            services.ConfigureMvc();
            services.ConfigureGlobalization();

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider, AppDbContext dbContext)
        {
            app.UseDeveloperExceptionPage();
            //app.UseHsts();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", 
                        description.GroupName.ToUpperInvariant());
                }
                c.ConfigObject.AdditionalItems.Add("persistAuthorization","true");
            });

            app.UseHttpsRedirection();
           
            app.UseRouting();
            app.UseCors("CorsPolicy");
            app.UseAuthentication();
            app.UseAuthorization();

            //app.UseMiddleware<SessionCheckerMiddleware>();

            app.UseErrorHandler();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            WebHelper.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());
        }
    }
}