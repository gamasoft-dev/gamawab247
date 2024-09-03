using System.Text.Json;
using Application.AutofacDI;
using Application.Services.Implementations;
using Application.Services.Interfaces;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using BroadcastMessageServiceWorker;
using BroadcastMessageServiceWorker.Services;

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
        services.AddSingleton<IBroadcastDispatchService, BroadcastDispatchService>();
        services.AddSingleton<IOutboundMesageService, OutboundMessageService>();
        services.AddSingleton<IMailService, MailService>();
        services.AddSingleton<IEmailTemplateService, EmailTemplateService>();
  }).Build();

await host.RunAsync();

