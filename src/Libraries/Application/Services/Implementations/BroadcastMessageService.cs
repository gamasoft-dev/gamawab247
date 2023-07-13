using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.DTOs.BusinessDtos;
using Application.Helpers;
using Application.Services.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Common;
using Domain.Entities;
using Infrastructure.Repositories.Implementations;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Application.Services.Implementations
{
    public class BroadcastMessageService : IBroadcastMessageService
    {
        private readonly IRepository<BroadcastMessage> _broadcastMessageRepo;
        private readonly IRepository<BusinessMessageSettings> _businessMessageSettingsRepo;
        private readonly IRepository<Business> _businessRepo;
        private readonly IMapper _mapper;
        public BroadcastMessageService(IRepository<BroadcastMessage> broadcastMessageRepo, 
            IMapper mapper, IRepository<BusinessMessageSettings> businessMessageSettingsRepo, IRepository<Business> businessRepo)
        {
            _broadcastMessageRepo = broadcastMessageRepo;
            _mapper = mapper;
            _businessMessageSettingsRepo = businessMessageSettingsRepo;
            _businessRepo = businessRepo;
        }

        public async Task<SuccessResponse<BroadcastMessageDto>> CreateBroadcastMessage(CreateBroadcastMessageDto model)
        {
            Guid businessId = Guid.Empty;
            if (model is null)
                throw new RestException(HttpStatusCode.BadRequest, "validate broadcast message");
            if (!string.IsNullOrEmpty(model.ApiKey))
            {
                var business = await _businessMessageSettingsRepo.FirstOrDefaultNoTracking(x => x.ApiKey == model.ApiKey)
                ?? throw new RestException(HttpStatusCode.NotFound, $"unable to retrieve business for ApiKey{model.ApiKey}");
                businessId = business.BusinessId;
            }else if (!string.IsNullOrEmpty(model.From))
            {
                var business = await _businessRepo.FirstOrDefaultNoTracking(x => x.PhoneNumber == model.From)
                ?? throw new RestException(HttpStatusCode.NotFound, $"unable to retrieve business for ApiKey{model.ApiKey}");
                businessId = business.Id;
            }

            if (businessId == Guid.Empty)
            {
                throw new RestException(HttpStatusCode.NotFound, $"unable to retrieve business for ApiKey");
            }
            var broadcastMessage = _mapper.Map<BroadcastMessage>(model);
            broadcastMessage.BusinessId = businessId;
            broadcastMessage.Status = Domain.Enums.EBroadcastMessageStatus.Pending;

            await _broadcastMessageRepo.AddAsync(broadcastMessage);
            await _broadcastMessageRepo.SaveChangesAsync();

            var response = _mapper.Map<BroadcastMessageDto>(broadcastMessage);

            return new SuccessResponse<BroadcastMessageDto>
            {
                Data = response,
                Message = ResponseMessages.Successful
            };
            
        }

        public async Task<SuccessResponse<BroadcastMessageDto>> UpdateBroadcastMessage(Guid id, UpdateBroadcastMessageDto model)
        {
            var broadcastMessage = await _broadcastMessageRepo.FirstOrDefault(x => x.Id == id)
                ?? throw new RestException(HttpStatusCode.NotFound, "unable to retrieve broadcast message");

            var message = _mapper.Map<BroadcastMessage>(model);
            _broadcastMessageRepo.Update(message);
            await _broadcastMessageRepo.SaveChangesAsync();

            var response = _mapper.Map<BroadcastMessageDto>(message);
            return new SuccessResponse<BroadcastMessageDto>
            {
                Data = response,
                Message = ResponseMessages.CreationSuccessResponse
            };
        }

        public async Task<SuccessResponse<bool>> DeleteBroadcastMessage(Guid id)
        {
            var broadcastMessage = await _broadcastMessageRepo.FirstOrDefault(x => x.Id == id)
                ?? throw new RestException(HttpStatusCode.NotFound, "unable to fetch broadcast message for the given id");

             _broadcastMessageRepo.Remove(broadcastMessage);
            await _broadcastMessageRepo.SaveChangesAsync();
            return new SuccessResponse<bool>
            {
                Data = true,
                Message = ResponseMessages.Successful
            };

        }

        public async Task<PagedResponse<IEnumerable<BroadcastMessageDto>>> GetAllBroadcastMessage(ResourceParameter parameter, string name, IUrlHelper urlHelper)
        {
            var queryable = _broadcastMessageRepo
                 .Query(x => (string.IsNullOrEmpty(parameter.Search)
                             || (x.Message.ToLower().Contains(parameter.Search.ToLower())|| x.Status.ToString().ToLower().Contains(parameter.Search))));

            var queryProjection = queryable.ProjectTo<BroadcastMessageDto>(_mapper.ConfigurationProvider);

            var broadcastMessages = await PagedList<BroadcastMessageDto>.CreateAsync(queryProjection, parameter.PageNumber, parameter.PageSize, parameter.Sort);
            var page = PageUtility<BroadcastMessageDto>.CreateResourcePageUrl(parameter, name, broadcastMessages, urlHelper);

            var response = new PagedResponse<IEnumerable<BroadcastMessageDto>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = broadcastMessages,
                Meta = new Helpers.Meta
                {
                    Pagination = page
                }
            };
            return response;
        }

        public async Task<PagedResponse<IEnumerable<BroadcastMessageDto>>> GetBroadcastMessageByBusinessId(Guid businessId, ResourceParameter parameter, string name, IUrlHelper urlHelper)
        {
            var queryable = _broadcastMessageRepo
                .Query(x => x.BusinessId == businessId);
            queryable = queryable.Where(x => x.Message.ToLower().Contains(parameter.Search.ToLower())
            || x.Status.ToString().ToLower().Contains(parameter.Search));

            var queryProjection = queryable.ProjectTo<BroadcastMessageDto>(_mapper.ConfigurationProvider);

            var broadcastMessages = await PagedList<BroadcastMessageDto>.CreateAsync(queryProjection, parameter.PageNumber, parameter.PageSize, parameter.Sort);
            var page = PageUtility<BroadcastMessageDto>.CreateResourcePageUrl(parameter, name, broadcastMessages, urlHelper);

            var response = new PagedResponse<IEnumerable<BroadcastMessageDto>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = broadcastMessages,
                Meta = new Helpers.Meta
                {
                    Pagination = page
                }
            };
            return response;
        }

        public async Task<SuccessResponse<BroadcastMessageDto>> GetBroadcastMessageById(Guid id)
        {
            var broadcastMessage = await _broadcastMessageRepo.FirstOrDefault(x => x.Id == id)
                ?? throw new RestException(HttpStatusCode.NotFound, "unable to retrieve broadcast message with the provided id");
            var response = _mapper.Map<BroadcastMessageDto>(broadcastMessage);

            return new SuccessResponse<BroadcastMessageDto>
            {
                Data = response,
                Message = ResponseMessages.RetrievalSuccessResponse
            };
        }

        public async Task<PagedResponse<IEnumerable<BroadcastMessageDto>>> GetAllPendingBroadcastMessage(ResourceParameter parameter, string name, IUrlHelper urlHelper)
        {
            var broadcastPendingBroadcastMessage = _broadcastMessageRepo
                .Query(x => x.Status == Domain.Enums.EBroadcastMessageStatus.Pending).OrderBy(x => x.CreatedAt);


            var queryProjection = broadcastPendingBroadcastMessage.ProjectTo<BroadcastMessageDto>(_mapper.ConfigurationProvider);

            var broadcastMessages = await PagedList<BroadcastMessageDto>.CreateAsync(queryProjection, parameter.PageNumber, parameter.PageSize, parameter.Sort);
            var page = PageUtility<BroadcastMessageDto>.CreateResourcePageUrl(parameter, name, broadcastMessages, urlHelper);

            var response = new PagedResponse<IEnumerable<BroadcastMessageDto>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = broadcastMessages,
                Meta = new Helpers.Meta
                {
                    Pagination = page
                }
            };
            return response;
        }


    }
}
