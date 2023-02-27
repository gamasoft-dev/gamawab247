using System.Threading.Tasks;
using Application.AutofacDI;
using Application.DTOs;
using Application.DTOs.InboundMessageDto;
using Application.Helpers;
using Domain.Enums;

namespace Application.Services.Interfaces.IReceiveMessageManager
{
    public interface IReceiveMessageManager : IAutoDependencyService
    {
        ReceiveMessageType MessageType { get; }
        Task<(string responseMessage, bool isSuccessful)> ProcessMessageByType(TextNotificationDto textType);
    }
}
