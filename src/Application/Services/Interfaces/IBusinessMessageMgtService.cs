using System;
using System.Threading.Tasks;
using Application.DTOs.CreateDialogDtos;
using Application.Helpers;
using Domain.Entities;
using Domain.Entities.DialogMessageEntitties;

namespace Application.Services.Interfaces;

public interface IBusinessMessageMgtService<TRequest, TResponse> 
    where TResponse : class where TRequest: class 
{
    string GetMessageType { get; }
    Task<SuccessResponse<BusinessMessageDto<TResponse>>> 
        CreateMessage(CreateBusinessMessageDto<TRequest> model);

    Task<(SuccessResponse<BusinessMessage> response, bool isTriggerForm)> GetNextBusinessMessageByOptionId(BaseInteractiveDto interactiveMessage,
        Guid businessId, string option, Guid? formId = null);
    Task<SuccessResponse<BusinessMessageDto<TResponse>>> GetBusinessMessageById(Guid id);

    Task<TResponse> GetInteractiveMessageById(Guid interactiveMessageId,
        string messageType);
    
    Task<SuccessResponse<bool>> HttpSendBusinessMessage(string wa_Id,
        BusinessMessageDto<BaseInteractiveDto> model, InboundMessage inboundMessage);
}