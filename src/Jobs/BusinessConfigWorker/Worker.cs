using Application.Cron;
using Application.DTOs;
using Microsoft.Extensions.Options;

namespace BusinessConfigWorker
{
    public class Worker : BackgroundService
    {
        private readonly IBusinessWebhookConfigRegistrationCron _businessWebhookConfigRegistrationCron;
        private readonly SystemSettingsConfig _config;

        public Worker(IBusinessWebhookConfigRegistrationCron businessWebhookConfigRegistrationCron, IOptions<SystemSettingsConfig> options)
        {
            _businessWebhookConfigRegistrationCron = businessWebhookConfigRegistrationCron;
            _config = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                try
                {
                    await _businessWebhookConfigRegistrationCron.ProcessGamaSoftBusinessConfigurationTo360();

                    await Task.Delay(_config.MessageResponseInMilliseconds, CancellationToken.None);
                }

                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    continue;
                }
            }
        }
    }
}