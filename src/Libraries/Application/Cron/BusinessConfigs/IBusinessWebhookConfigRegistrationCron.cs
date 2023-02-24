using Application.AutofacDI;
using Application.Helpers;
using System.Threading.Tasks;

namespace Application.Cron
{
    public interface IBusinessWebhookConfigRegistrationCron : IAutoDependencyService
    {
        Task ProcessGamaSoftBusinessConfigurationTo360();
    }
}