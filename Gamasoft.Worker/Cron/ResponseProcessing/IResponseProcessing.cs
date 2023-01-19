using System.Threading.Tasks;
using Application.Helpers;

namespace Gamasoft.Worker.Cron.ResponseProcessing;

public interface IResponsePreProcessing: IAutoDependencyService
{
    Task InitiateMessageProcessing();
}