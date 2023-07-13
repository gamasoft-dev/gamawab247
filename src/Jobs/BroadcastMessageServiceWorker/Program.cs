using System.Text.Json;
using Application.AutofacDI;
using Application.Services.Implementations;
using Application.Services.Interfaces;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using BroadcastMessageServiceWorker;
using BroadcastMessageServiceWorker.Services;
using Microsoft.AspNetCore.Hosting;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
{
    webBuilder.UseStartup<Startup>();

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
  }).Build();

await host.RunAsync();

