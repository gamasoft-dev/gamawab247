using System;
using System.Threading.Tasks;
using Application.DTOs.CreateDialogDtos;
using Application.AutofacDI;
using Application.Helpers;

namespace Application.Services.Interfaces;

public interface IBusinessMessageService: IAutoDependencyService
{
    Task<SuccessResponse<BusinessMessageDto<T>>> CreateMessage<T, TRequest>(CreateBusinessMessageDto<TRequest> model) where T: BaseInteractiveDto;
    // Task<SuccessResponse<BusinessMessageDto<T>>> CreateListMessage<T>(CreateBusinessMessageDto<T> model)where T: BaseInteractiveDto;
    // Task<SuccessResponse<BusinessMessageDto<T>>> CreateTextMessage<T>(CreateBusinessMessageDto<T> model) where T: BaseInteractiveDto;
    // Task<SuccessResponse<BusinessMessageDto<T>>> GetBusinessMessageById<T>(Guid id) where T: BaseInteractiveDto;
    Task<SuccessResponse<BusinessMessageDto<T>>> GetBusinessMessageByReplyId<T>(Guid buttonMessageTypeId);
    Task<SuccessResponse<BusinessMessageDto<T>>> GetBusinessMessageByListId<T>(Guid listMessageTypeId);
    Task<SuccessResponse<bool>> UpdateTextMessage(Guid businessMessageId, UpdateBusinessMessageDto<UpdateTextMessageDto> model);
    Task<SuccessResponse<bool>> UpdateListMessage(Guid businessMessageId, UpdateBusinessMessageDto<UpdateListMessageDto> model);
}