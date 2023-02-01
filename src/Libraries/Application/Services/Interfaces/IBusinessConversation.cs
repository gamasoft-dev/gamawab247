using Application.DTOs.BusinessDtos;
using Application.DTOs.CreateDialogDtos;
using Application.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
	public interface IBusinessConversation
	{
		Task<SuccessResponse<BusinessConversationDto>> CreateMessage(CreateBusinessConversationDtoo createbusinessConversation);
		Task<SuccessResponse<BusinessConversationDto>> GetBusinessConversationByBusinessId(Guid businessId);
		Task<PagedResponse<IEnumerable<BusinessConversationDto>>> GetAllBusinessConversations(ResourceParameter parameter, string endPointName, IUrlHelper url);
		Task<SuccessResponse<bool>> UpdateBusinessConversation(UpdateBusinessConversationDto businessConversation);
		Task<SuccessResponse<bool>> DeleteBusinessConversation(Guid Id);
		//Task<SuccessResponse<BusinessMessageDto<T>>> GetBusinessMessageByListId<T>(Guid listMessageTypeId);
		//Task<SuccessResponse<bool>> UpdateTextMessage(Guid businessMessageId, UpdateBusinessMessageDto<UpdateTextMessageDto> model);
		//Task<SuccessResponse<bool>> UpdateListMessage(Guid businessMessageId, UpdateBusinessMessageDto<UpdateListMessageDto> model);
	}
}
