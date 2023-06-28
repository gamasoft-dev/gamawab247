using BroadcastMessageServiceWorker;
using BroadcastMessageServiceWorker.Services;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddScoped<IBroadcastDispatchService, BroadcastDispatchService>();
    })
    .Build();

await host.RunAsync();

