using Application.AutofacDI;
using Application.Helpers;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;

namespace Gamasoft.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseSentry(o => {
                        o.Dsn = "https://3980744645414fc68df06ec33fd9abe7@o1289200.ingest.sentry.io/6507272";
                        o.Debug = true;
                        o.TracesSampleRate = 1.0;
                        o.Environment = "production";
                    });
                    //webBuilder.UseUrls("https://*:8081", "http://*:8080");
                })
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>(builder =>
                {
                    builder.RegisterModule(new AutofacContainerModule());
                });
    }
}