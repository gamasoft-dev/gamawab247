using System.Threading.Tasks;
using Application.Helpers;
using Domain.Entities;

namespace Application.Cron.ResponseProcessing;

public interface IResponsePreProcessingCron: IAutoDependencyService
{
    Task InitiateMessageProcessing();

    Task SendUnResolvedMessage(InboundMessage inboundMessage, string Message);
}