using System;
using System.Threading.Tasks;
using Application.DTOs.CreateDialogDtos;
using Application.Helpers;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Entities.DialogMessageEntitties;

namespace Application.Services.Implementations.BusinessMessageImpls;

public abstract class BusinessMessageServiceAbs:
    IBusinessMessageMgtService<BaseCreateMessageDto, BaseInteractiveDto> 
{
    public virtual string GetMessageType { get; }

    public abstract Task<SuccessResponse<BusinessMessageDto<BaseInteractiveDto>>>
        CreateMessage(CreateBusinessMessageDto<BaseCreateMessageDto> model);

    public abstract Task<(SuccessResponse<BusinessMessage> response, bool isTriggerForm)>
        GetNextBusinessMessageByOptionId(BaseInteractiveDto interactiveMessage, Guid businessId, string option, Guid? formId = null);

    public abstract Task<SuccessResponse<BusinessMessageDto<BaseInteractiveDto>>>
        GetBusinessMessageById(Guid id);
    public abstract Task<BaseInteractiveDto> GetInteractiveMessageById(Guid interactiveMessageId, string messageType);

    public abstract Task<SuccessResponse<bool>> HttpSendBusinessMessage(string wa_Id,
        BusinessMessageDto<BaseInteractiveDto> model, InboundMessage inboundMessage);

}