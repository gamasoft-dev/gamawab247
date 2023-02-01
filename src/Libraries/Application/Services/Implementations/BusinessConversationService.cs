using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities.DialogMessageEntitties.ValueObjects;
using Domain.Entities.DialogMessageEntitties;
using Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Helpers;
using Application.DTOs.BusinessDtos;
using Application.DTOs.InteractiveMesageDto;
using Domain.Common;
using System.Net;
using Application.DTOs.CreateDialogDtos;
using Microsoft.AspNetCore.Mvc;
using AutoMapper.QueryableExtensions;
using Domain.Entities;

namespace Application.Services.Implementations
{
    public class BusinessConversationService: IBusinessConversation
    {
		private readonly IMapper _mapper;
		private readonly IRepository<BusinessMessage> _businessMessageRepository;
		private readonly IRepository<BusinessConversation> _businessConversationRepo;

		public BusinessConversationService(IMapper mapper,
			IRepository<BusinessMessage> businessMessageRepository,
			IRepository<BusinessConversation> businessConversationRepo)
		{
			_mapper = mapper;
			_businessMessageRepository = businessMessageRepository;
			_businessConversationRepo = businessConversationRepo;
	
		}

		public async Task<SuccessResponse<BusinessConversationDto>> CreateMessage(CreateBusinessConversationDtoo createBusinessConversation)
		{
			var businessConversation = _mapper.Map<BusinessConversation>(createBusinessConversation);
			if (businessConversation.Id == null ||  string.IsNullOrEmpty(businessConversation.Title))
				throw new RestException(HttpStatusCode.BadRequest, ResponseMessages.BusinessConversationNotFound);

			// add business conversation and generate Id
			await _businessConversationRepo.AddAsync(businessConversation);
			await _businessConversationRepo.SaveChangesAsync();
			var result = _mapper.Map<BusinessConversationDto>(businessConversation);
			return new SuccessResponse<BusinessConversationDto>
			{
				Data = result,
				Message = "Successfully created",
				Success = true
			};
		}

		public async Task<SuccessResponse<bool>> DeleteBusinessConversation(Guid Id)
		{
			if (Id != Guid.Empty)
			{
				var getBusinessConversation = await _businessConversationRepo.FirstOrDefault(x=>x.Id == Id);
				if (getBusinessConversation != null)
					_businessConversationRepo.Remove(getBusinessConversation);
			}
			await _businessConversationRepo.SaveChangesAsync();

			return new SuccessResponse<bool>
			{
				Message = ResponseMessages.Successful
			};
		}

		public async Task<PagedResponse<IEnumerable<BusinessConversationDto>>> GetAllBusinessConversations(ResourceParameter parameter, string endPointName, IUrlHelper url)
		{
			var queryable = _businessConversationRepo
				.Query(x => (string.IsNullOrEmpty(parameter.Search) || (x.Title.ToLower().Contains(parameter.Search.ToLower()))));

			var queryProjection = queryable.ProjectTo<BusinessConversationDto>(_mapper.ConfigurationProvider);
			var businessConversationMessages = await PagedList<BusinessConversationDto>.CreateAsync(queryProjection, parameter.PageNumber,parameter.PageSize, parameter.Sort);
			var page = PageUtility<BusinessConversationDto>.CreateResourcePageUrl(parameter, endPointName, businessConversationMessages, url);

			var response = new PagedResponse<IEnumerable<BusinessConversationDto>>
			{
				Message = ResponseMessages.RetrievalSuccessResponse,
				Data = businessConversationMessages,
				Meta = new Helpers.Meta
				{
					Pagination = page
				}
			};
			return response;
		}

		public async Task<SuccessResponse<BusinessConversationDto>> GetBusinessConversationByBusinessId(Guid Id)
		{
			var retrieveBusinessConversation = _businessConversationRepo.FirstOrDefault(x=>x.Id == Id);
			if (retrieveBusinessConversation == null) throw new RestException(HttpStatusCode.BadRequest, ResponseMessages.BusinessConversationNotFoundForId);

			var businessConversation = _mapper.Map<BusinessConversationDto>(retrieveBusinessConversation);
			return new SuccessResponse<BusinessConversationDto>
			{
				Data = businessConversation,
				Message = "Successfully retrieved",
				Success = true
			};
		}

		public async Task<SuccessResponse<bool>> UpdateBusinessConversation(UpdateBusinessConversationDto businessConversation)
		{
			if (businessConversation == null || businessConversation.Id == Guid.Empty) 
				throw new RestException(HttpStatusCode.BadRequest, ResponseMessages.BusinessConversationNotFoundForId);

			var getBusinessConversation = await _businessConversationRepo.FirstOrDefault(x => x.Id == businessConversation.Id);
			if (getBusinessConversation == null)
				throw new RestException(HttpStatusCode.BadRequest, ResponseMessages.Failed);

			getBusinessConversation.Title = businessConversation.Title;
			getBusinessConversation.UpdatedAt = DateTime.Now;

			_businessConversationRepo.Update(getBusinessConversation);
			await _businessConversationRepo.SaveChangesAsync();

			return new SuccessResponse<bool>
			{
				Message = ResponseMessages.UpdateResponse
			};
		}

		
	}
}
