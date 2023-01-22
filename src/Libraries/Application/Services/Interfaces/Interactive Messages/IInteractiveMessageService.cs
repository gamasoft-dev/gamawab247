using System;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Helpers;
using Domain.Enums;

namespace Application.Services.Interfaces.Interactive_Messages
{
    public interface IInteractiveMessageProvider: IAutoDependencyService
    {
        EMessageType MessageType { get; }
        Task<SuccessResponse<IInteractiveMessageResponse>> CreateMessage<T>(Guid businessId, T model);
        Task<IInteractiveMessageResponse> ReceiveMessage<T>(T model);
        Task<IInteractiveMessageResponse> GetById(Guid businessMessageId, string recipient = null);
    }
}
