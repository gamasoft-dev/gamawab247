using Application.DTOs;
using Application.Helpers;
using Application.Services.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Common;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.Implementations
{
	//TODO: Create implementations for the respective conversation methods.
	public class IndustryConversationFlowService : IIndustryConversationFlowService
	{
		private readonly IRepository<IndustryConversationFlow> _convoRepository;
		private readonly IMapper _mapper;
		public IndustryConversationFlowService(
			IRepository<IndustryConversationFlow> convoRepository,
			IMapper mapper)
		{
			_convoRepository = convoRepository;
			_mapper = mapper;
		}

		/// <summary>
		/// Create conversation flow
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task<SuccessResponse<GetIndustryConversationFlowDto>> 
			CreateConversationFlow(CreateIndustryConversationFlowDto model)
		{
			// validate keyword count. // should not be more than 3 keywords for a converstion flow record
			if(model.RequestKeywords.Split(",").Count() > 3)
				throw new RestException(System.Net.HttpStatusCode.BadRequest, 
					"The conversation request keywords must not exceed 3 keywords, for concise coversational response.");

			// Check if there is already a conversation for the subject for the business
			var conversationFlowBySubject = await _convoRepository.FirstOrDefaultNoTracking(x => x.SubjectMatter == model.SubjectMatter);
			if (conversationFlowBySubject is not null)
				throw new RestException(System.Net.HttpStatusCode.BadRequest,
					"There is already a conversation flow for this subject matter. You can retrieve list of conversation by subject matter and update that conversation");

			var conversation = _mapper.Map<IndustryConversationFlow>(model);
			conversation.Response = model.RequestKeywords;
			await _convoRepository.AddAsync(conversation);

			await _convoRepository.SaveChangesAsync();

			var response = await GetById(conversation.Id);

			return new SuccessResponse<GetIndustryConversationFlowDto>
			{
				Data = response.Data,
				Message = ResponseMessages.CreationSuccessResponse
			};
		}

		/// <summary>
		/// Update Conversation flow.
		/// </summary>
		/// <param name="industryId"></param>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task<SuccessResponse<GetIndustryConversationFlowDto>> 
			UpdateConversationFlow(Guid industryId, UpdateIndustryConversationFlowDto model)
		{
			// find the conversation by id.
			var conversation = await _convoRepository.FirstOrDefault(x => x.Id == industryId);

			// update the response 
			if (conversation is null)
				throw new RestException(System.Net.HttpStatusCode.NotFound, "No conversation found");

			// update subject 
			_mapper.Map(conversation, model);

			await _convoRepository.SaveChangesAsync();

			var updateConversation = await GetById(id: industryId);

			return new SuccessResponse<GetIndustryConversationFlowDto>
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

		/// <summary>
		/// Get a conversation by id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public async Task<SuccessResponse<GetIndustryConversationFlowDto>> GetById(Guid id)
		{
			var conversation = await _convoRepository.GetByIdAsync(id);

			if (conversation is null)
				throw new RestException(System.Net.HttpStatusCode.NotFound, "Conversation not found");

			var conversationDto = _mapper.Map<GetIndustryConversationFlowDto>(conversation);

			return new SuccessResponse<GetIndustryConversationFlowDto>
			{
				Data = conversationDto,
				Message = ResponseMessages.RetrievalSuccessResponse
			};
		}

		/// <summary>
		/// Get conversation flows as a paginated response.
		/// </summary>
		/// <param name="parameter"></param>
		/// <returns></returns>
		public async Task<PagedList<GetIndustryConversationFlowDto>> GetConversationFlows(ResourceParameter parameter)
		{
			var convoQuery = _convoRepository.Query((x => x.RequestKeywords.Contains(parameter.Search)
			|| x.Response.Contains(parameter.Search) || x.SubjectMatter.Contains(parameter.Search)))
				.Skip(parameter.Skip)
				.Take(parameter.Take);

			var convoResponse = convoQuery.ProjectTo<GetIndustryConversationFlowDto>(_mapper.ConfigurationProvider);
			var paggedResponse = await PagedList<GetIndustryConversationFlowDto>.CreateAsync(convoResponse, 
				parameter.PageNumber, 
				parameter.PageSize,
				parameter.Sort);

			return paggedResponse;
		}	
    }
}