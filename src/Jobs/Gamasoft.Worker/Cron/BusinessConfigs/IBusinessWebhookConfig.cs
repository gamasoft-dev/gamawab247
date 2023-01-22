using Application.Helpers;
using System.Threading.Tasks;

namespace Gamasoft.Worker.Cron.BusinessConfigs
{
    public interface IBusinessWebhookConfig : IAutoDependencyService
    {
        Task ProcessGamaSoftBusinessConfigurationTo360();
    }
}