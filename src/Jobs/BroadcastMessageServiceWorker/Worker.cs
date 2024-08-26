using Application.DTOs;
using BroadcastMessageServiceWorker.Services;
using Microsoft.Extensions.Options;

namespace BroadcastMessageServiceWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IBroadcastDispatchService _broadcastDispatch;
    private readonly SystemSettingsConfig _config;

    public Worker(ILogger<Worker> logger,
        IBroadcastDispatchService broadcastDispatch,
        IOptions<SystemSettingsConfig> config)
    {
        _logger = logger;
        _broadcastDispatch = broadcastDispatch;
        _config = config.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(_config.BgDelayIntervalMilliseconds, CancellationToken.None);
            _logger.LogInformation("Worker starting process at: {time}", DateTimeOffset.Now);

            try
            {

                await _broadcastDispatch.SendMessage();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                continue;
            }
            _logger.LogInformation("Worker ending process at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
        
        _logger.LogInformation("Background Job exiting and closing down at: {time}", DateTimeOffset.Now);

    }
}

