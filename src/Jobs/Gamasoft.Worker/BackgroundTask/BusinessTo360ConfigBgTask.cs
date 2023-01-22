using Application.Cron;
using Application.DTOs;
using Gamasoft.Worker.Cron.BusinessConfigs;
using Microsoft.Extensions.Options;

namespace Gamasoft.Worker.BackgroundTask
{
    public class BusinessTo360ConfigBgTask : BackgroundService
    {
        private readonly IBusinessWebhookConfigRegistrationCron _businessWebhookConfig;
        private readonly ILogger<BusinessTo360ConfigBgTask> _logger;
        private readonly SystemSettingsConfig _config;
        public BusinessTo360ConfigBgTask(IBusinessWebhookConfigRegistrationCron businessWebhookConfig,
            ILogger<BusinessTo360ConfigBgTask> logger, IOptions<SystemSettingsConfig> options)
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
                    await Task.Delay(_config.WebhookConfigTo360InMilliseconds, stoppingToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(errorMessage + " " + $"{DateTime.UtcNow.ToString()} {e.Message}");
                }
            }
        }
    }
}