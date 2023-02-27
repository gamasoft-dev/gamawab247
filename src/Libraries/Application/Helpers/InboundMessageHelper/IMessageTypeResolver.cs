using System.Threading.Tasks;
using Application.AutofacDI;
using Application.DTOs;
using Application.DTOs.InboundMessageDto;
using Domain.Enums;

namespace Application.Helpers.InboundMessageHelper
{
    public interface IMessageTypeResolver : IAutoDependencyService
    {
        Task<string> GetMessageType(TextNotificationDto inboundText);
    }
}