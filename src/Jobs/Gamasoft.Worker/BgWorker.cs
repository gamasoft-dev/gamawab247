using Application.Cron.ResponseProcessing;
using Application.DTOs;
using Microsoft.Extensions.Options;
using static System.DateTime;

namespace Gamasoft.Worker.BackgroundTask;

public class BgWorker : BackgroundService
{
    private readonly IResponsePreProcessingCron _responsePreProcessing;
    private readonly ILogger<BgWorker> _logger;
    private readonly SystemSettingsConfig _config;

    public BgWorker(IResponsePreProcessingCron responsePreProcessing,
        ILogger<BgWorker> logger, IOptions<SystemSettingsConfig> options)
    {
        _responsePreProcessing = responsePreProcessing;
        _logger = logger;
        _config = options.Value;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        const string errorMessage = "An error occurred on the background job at" ;
        while (true)
        {
            try
            {
                await _responsePreProcessing.InitiateMessageProcessing();

                await Task.Delay(_config.MessageResponseInMilliseconds, stoppingToken);
            }
            catch (Exception e)
            {
               _logger.LogError(errorMessage + " " + $"{UtcNow.ToString()} {e.Message}" );
            }
        }
    }
}