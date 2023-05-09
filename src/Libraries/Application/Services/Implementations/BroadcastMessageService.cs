using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Helpers;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Application.Services.Implementations
{
    public class BroadcastMessageService : IBroadcastMessageService
    {
        private readonly IRepository<BroadcastMessage> _broadcastMessageRepo;
        private readonly IRepository<BusinessMessageSettings> _businessMessageSettingsRepo;
        private readonly IMapper _mapper;
        public BroadcastMessageService(IRepository<BroadcastMessage> broadcastMessageRepo, IMapper mapper, IRepository<BusinessMessageSettings> businessMessageSettingsRepo)
        {
            _broadcastMessageRepo = broadcastMessageRepo;
            _mapper = mapper;
            _businessMessageSettingsRepo = businessMessageSettingsRepo;
        }

        public async Task<SuccessResponse<BroadcastMessageDto>> CreateBroadcastMessage(CreateBroadcastMessageDto model)
        {
            if (model is null)
                throw new RestException(HttpStatusCode.BadRequest, "validate broadcast message");

            var business = await _businessMessageSettingsRepo.FirstOrDefaultNoTracking(x => x.ApiKey == model.ApiKey)
                ?? throw new RestException(HttpStatusCode.NotFound,$"unable to retrieve business for ApiKey{model.ApiKey}");

            var broadcastMessage = _mapper.Map<BroadcastMessage>(model);
            broadcastMessage.BusinessId = business.BusinessId;
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

            //var response = _mapper.Map<BroadcastMessage>()
                throw new NotImplementedException();
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

        public Task<PagedResponse<IEnumerable<BroadcastMessageDto>>> GetAllBroadcastMessage(ResourceParameter parameter, string name, IUrlHelper urlHelper)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResponse<IEnumerable<BroadcastMessageDto>>> GetBroadcastMessageByBusinessId(Guid businessId, ResourceParameter parameter, string name, IUrlHelper urlHelper)
        {
            throw new NotImplementedException();
        }

        public Task<SuccessResponse<BroadcastMessageDto>> GetBroadcastMessageById(Guid id)
        {
            throw new NotImplementedException();
        }

        
    }
}
