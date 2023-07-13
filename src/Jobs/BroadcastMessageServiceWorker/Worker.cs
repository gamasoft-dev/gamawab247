using Application.DTOs;
using BroadcastMessageServiceWorker.Services;
using Microsoft.Extensions.Options;

namespace BroadcastMessageServiceWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IBroadcastDispatchService _broadcastDispatch;
    private readonly SystemSettingsConfig _config;

    public Worker(ILogger<Worker> logger, IBroadcastDispatchService broadcastDispatch, IOptions<SystemSettingsConfig> config)
    {
        _logger = logger;
        _broadcastDispatch = broadcastDispatch;
        _config = config.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _broadcastDispatch.SendMessage();

                await Task.Delay(_config.MessageResponseInMilliseconds, CancellationToken.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                continue;
            }
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }
}

