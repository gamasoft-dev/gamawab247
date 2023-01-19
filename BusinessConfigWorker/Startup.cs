using Application.Helpers;
using BusinessConfigWorker.Middleware;
using BusinessConfigWorker.ServiceExtension;
using Infrastructure.Data.DbContext;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BusinessConfigWorker
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            var builder = new ConfigurationBuilder()
                .SetBasePath(hostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
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
            services.ConfigureLoggerService();
            services.ConfigureSqlContext(Configuration);
            services.ConfigureRedisCache(Configuration);
            services.AddAuthentication();
            services.AddHttpContextAccessor();
            services.ConfigureRepositoryManager();
            services.ConfigureIOObjects(Configuration);
            services.AddControllers()
                .AddXmlDataContractSerializerFormatters();
            //services.ConfigureMvcAndAutomapper();
            services.ConfigureGlobalization();
            services.ConfigureIdentity();

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            services.AddHostedService<Worker>();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AppDbContext dbContext)
        {
            app.UseDeveloperExceptionPage();


            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseErrorHandler();

            WebHelper.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());
        }
    }
}
