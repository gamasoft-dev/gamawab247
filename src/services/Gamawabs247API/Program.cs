using Application.AutofacDI;
using Application.Helpers;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Infrastructure.Data.DbContext;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace API;
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            
            //DbInitializer.SeedRoleData(services).Wait();
            //DbInitializer.SeedSuperUser(services).Wait();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseSentry(o => {
                        o.Dsn = "https://3980744645414fc68df06ec33fd9abe7@o1289200.ingest.sentry.io/6507272";
                        o.Debug = true;// Set TracesSampleRate to 1.0 to capture 100% of transactions for performance monitoring.// We recommend adjusting this value in production.
                        o.TracesSampleRate = 1.0;
                        o.Environment = "production";
                    });
                })
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>(builder =>
                {
                    builder.RegisterModule(new AutofacContainerModule());
                });
    }
