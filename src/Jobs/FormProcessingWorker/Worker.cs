using Application.DTOs;
using Application.Services.Cron;
using Microsoft.Extensions.Options;

namespace FormProcessingWorker;

public class Worker : BackgroundService
{
    private readonly IFormProcessorCron _formProcessorCron;
    private readonly SystemSettingsConfig _config;

    public Worker(IFormProcessorCron formProcessorCron, IOptions<SystemSettingsConfig> options)
    {
        _formProcessorCron = formProcessorCron;
        _config = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (true)
        {
            try
            {
                await _formProcessorCron.DoWork();

                await Task.Delay(_config.MessageResponseInMilliseconds, CancellationToken.None);
            }
            catch (Exception ex )
            {
                Console.WriteLine(ex.Message);
                continue;
            }
        }
    }
}

