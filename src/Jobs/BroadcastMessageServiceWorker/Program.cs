using System.Text.Json;
using Application.AutofacDI;
using Application.Services.Implementations;
using Application.Services.Interfaces;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using BillProcessorAPI.Repositories.Implementations;
using BillProcessorAPI.Repositories.Interfaces;
using BroadcastMessageServiceWorker;
using BroadcastMessageServiceWorker.Services;
using Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

IConfiguration configuration;
IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
{
    webBuilder.UseStartup<Startup>();

    //webBuilder.UseUrls("https://*:8015", "http://*:8082");
})
  .UseServiceProviderFactory(new AutofacServiceProviderFactory())
  .ConfigureContainer<ContainerBuilder>(builder =>
  {
      builder.RegisterModule(new AutofacContainerModule());
    

  })
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddScoped<IBroadcastDispatchService, BroadcastDispatchService>();
        services.AddScoped<IOutboundMesageService, OutboundMessageService>();
        services.AddScoped<IBillTransactionRepository, BillTransactionRespository>();
    })
    .Build();

await host.RunAsync();

