using Application.Cron;
using Application.DTOs;
using Microsoft.Extensions.Options;

namespace Gamasoft._360WebhookConfig
{
    public class Worker : BackgroundService
    {
        private readonly IBusinessWebhookConfigRegistrationCron _businessWebhookConfig;
        private readonly ILogger<Worker> _logger;
        private readonly SystemSettingsConfig _config;
        public Worker(IBusinessWebhookConfigRegistrationCron businessWebhookConfig,
            ILogger<Worker> logger, IOptions<SystemSettingsConfig> options)
        {
            _businessWebhookConfig = businessWebhookConfig;
            _logger = logger;
            _config = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            const string errorMessage = "An error occurred on the background job at";
            while (true)
            {
                try
                {
                    await _businessWebhookConfig.ProcessGamaSoftBusinessConfigurationTo360();
                    await Task.Delay(2000, stoppingToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(errorMessage + " " + $"{DateTime.UtcNow.ToString()} {e.Message}");
                }
            }
        }
    }
}