using System.Threading;
using System.Threading.Tasks;
using FormProcessingWorker.Services.Cron;
using Microsoft.Extensions.Hosting;

namespace API.BackgroundTask
{
    public class FormProcessorBgTask: BackgroundService
    {
        private readonly IFormProcessorCron _formProcessorCron;

        public FormProcessorBgTask(IFormProcessorCron formProcessorCron)
        {
            _formProcessorCron = formProcessorCron;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                await _formProcessorCron.DoWork();

                await Task.Delay(2000, CancellationToken.None);
            }
        }
    }
}

