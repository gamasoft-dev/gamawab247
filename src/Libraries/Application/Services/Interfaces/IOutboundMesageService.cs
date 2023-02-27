using Application.Helpers;
using System.Threading.Tasks;
using Application.DTOs.CreateDialogDtos;
using Domain.Entities;
using Domain.Entities.FormProcessing;
using Application.AutofacDI;

namespace Application.Services.Interfaces
{
	public interface IOutboundMesageService: IAutoDependencyService
	{ 
		Task<SuccessResponse<bool>> HttpSendReplyButtonMessage(string wa_Id,
			BusinessMessageDto<BaseInteractiveDto> model, InboundMessage inboundMessage);
		Task<SuccessResponse<bool>> HttpSendListMessage(string wa_Id,
			BusinessMessageDto<BaseInteractiveDto> model, InboundMessage inboundMessage);
		Task<SuccessResponse<bool>> HttpSendTextMessage(string wa_Id,
			BusinessMessageDto<BaseInteractiveDto> model, InboundMessage inboundMessage);

		Task<SuccessResponse<bool>> HttpSendTextMessage(string wa_Id, FormRequestResponse model);

		Task<SuccessResponse<bool>> SendMessage(string messageType, string wa_Id,
			BusinessMessageDto<BaseInteractiveDto> model, InboundMessage message);
	}
}
