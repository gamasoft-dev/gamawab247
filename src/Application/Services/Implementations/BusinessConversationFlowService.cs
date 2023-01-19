using Application.DTOs;
using Application.Helpers;
using Application.Services.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Common;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Application.Services.Implementations
{
	//TODO: Create implementation for the respective methods
	public class BusinessConversationFlowService : IBusinessConversationFlowService
	{
		private readonly IRepository<BusinessConversationFlow> _convoRepository;
		private readonly IMapper _mapper;

		public BusinessConversationFlowService(IRepository<BusinessConversationFlow> convoRepository,
			IMapper mapper)
		{
			_convoRepository = convoRepository;
			_mapper = mapper;
		}

		#region Command Seervices
		public async Task<SuccessResponse<GetBusinessConversationFlowDto>> CreateConversationFlow(Guid businessId, CreateBusinessConversationFlowDto model)
		{
			if (model.RequestKeywords.Split(",").Count() > 3)
				throw new RestException(System.Net.HttpStatusCode.BadRequest,
					"The conversation request keywords must not exceed 3 keywords, for concise coversational response.");

			// Check if there is already a conversation for the subject for the business
			var conversationFlowBySubject = await _convoRepository.FirstOrDefaultNoTracking(x => x.SubjectMatter == model.SubjectMatter);
			if (conversationFlowBySubject is not null)
				throw new RestException(System.Net.HttpStatusCode.BadRequest,
					"There is already a conversation flow for this subject matter. You can retrieve list of conversation by subject matter and update that conversation");

			var conversation = _mapper.Map<BusinessConversationFlow>(model);
			conversation.Response = model.RequestKeywords;
			conversation.BusinessId = businessId;
			await _convoRepository.AddAsync(conversation);

			await _convoRepository.SaveChangesAsync();

			var response = await GetById(conversation.Id);

			return new SuccessResponse<GetBusinessConversationFlowDto>
			{
				Data = response.Data,
				Message = ResponseMessages.CreationSuccessResponse
			};
		}

		public async Task<SuccessResponse<GetBusinessConversationFlowDto>> UpdateConversationFlow(Guid id, UpdateBusinessConversationFlowDto model)
		{
			// find the conversation by id.
			var conversation = await _convoRepository.FirstOrDefault(x => x.Id == id);

			// update the response 
			if (conversation is null)
				throw new RestException(System.Net.HttpStatusCode.NotFound, "No conversation found");

			// update subject 
			_mapper.Map(conversation, model);

			await _convoRepository.SaveChangesAsync();

			var updateConversation = await GetById(id: id);

			return new SuccessResponse<GetBusinessConversationFlowDto>
			{
				Data = updateConversation.Data,
				Message = ResponseMessages.UpdateResponse
			};
		}
		public async Task Delete(Guid id)
		{
			// find conversation by id
			var industryConversation = await _convoRepository.GetByIdAsync(id);

			// delete if exist
			if (industryConversation is null)
				throw new RestException(System.Net.HttpStatusCode.NotFound, "This conversation does not exist");

			_convoRepository.Remove(industryConversation);
			await _convoRepository.SaveChangesAsync();
		}

		#endregion

		#region Query services

		public async Task<SuccessResponse<GetBusinessConversationFlowDto>> GetById(Guid id)
		{
			var conversation = await _convoRepository.GetByIdAsync(id);

			if (conversation is null)
				throw new RestException(System.Net.HttpStatusCode.NotFound, "Conversation not found");

			var conversationDto = _mapper.Map<GetBusinessConversationFlowDto>(conversation);

			return new SuccessResponse<GetBusinessConversationFlowDto>
			{
				Data = conversationDto,
				Message = ResponseMessages.RetrievalSuccessResponse
			};
		}

		public async Task<PagedList<GetBusinessConversationFlowDto>> GetConversationFlows(ResourceParameter parameter)
		{
			var convoQuery = _convoRepository.Query(x => x.RequestKeywords.Contains(parameter.Search)
				|| x.Response.Contains(parameter.Search) || x.SubjectMatter.Contains(parameter.Search))
				.Skip(parameter.Skip)
				.Take(parameter.Take);

			var convoResponse = convoQuery.ProjectTo<GetBusinessConversationFlowDto>(_mapper.ConfigurationProvider);
			var paggedResponse = await PagedList<GetBusinessConversationFlowDto>.CreateAsync(convoResponse,
				parameter.PageNumber,
				parameter.PageSize,
				parameter.Sort);

			return paggedResponse;
		}

		public IEnumerable<BusinessConversationFlow> GetConversationFlows(Expression<Func<BusinessConversationFlow, bool>> expression,
			int skip = 10  , int take = 10)
		{
			var convoQuery = _convoRepository.Query(expression).
				Skip(skip).
				Take(take).OrderByDescending(x=>x.CreatedAt);

			return convoQuery.ToList();
		}

		public async Task<BusinessConversationFlow> FirstOrDefault(Expression<Func<BusinessConversationFlow, bool>> expression)
		{
			return await _convoRepository.Query(expression).FirstOrDefaultAsync(); 
		}
		#endregion
	}
}
