using Application.DTOs;
using Application.Helpers;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
	public interface IBusinessConversationFlowService: IAutoDependencyService
	{
		Task<SuccessResponse<GetBusinessConversationFlowDto>> CreateConversationFlow(Guid businessId, 
			CreateBusinessConversationFlowDto model);

		Task<SuccessResponse<GetBusinessConversationFlowDto>> UpdateConversationFlow(Guid id,
			UpdateBusinessConversationFlowDto model);

		Task Delete(Guid id);

		Task<SuccessResponse<GetBusinessConversationFlowDto>> GetById(Guid id);

		Task<PagedList<GetBusinessConversationFlowDto>> GetConversationFlows(ResourceParameter parameter);

		IEnumerable<BusinessConversationFlow> GetConversationFlows(Expression<Func<BusinessConversationFlow, bool>> expression,
			int skip = 10, int take = 10);
		Task<BusinessConversationFlow> FirstOrDefault(Expression<Func<BusinessConversationFlow, bool>> expression);
	}
}
