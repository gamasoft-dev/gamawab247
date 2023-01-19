using Application.DTOs;
using Application.Helpers;
using System;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
	public interface IIndustryConversationFlowService: IAutoDependencyService
	{
		Task<SuccessResponse<GetIndustryConversationFlowDto>> CreateConversationFlow(CreateIndustryConversationFlowDto model);

		Task<SuccessResponse<GetIndustryConversationFlowDto>> UpdateConversationFlow(Guid industryId, 
			UpdateIndustryConversationFlowDto model);
		
		Task Delete(Guid id);

		Task<SuccessResponse<GetIndustryConversationFlowDto>> GetById(Guid id);

		Task<PagedList<GetIndustryConversationFlowDto>> GetConversationFlows(ResourceParameter parameter);
	}
}
