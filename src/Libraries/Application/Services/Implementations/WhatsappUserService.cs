using Application.DTOs;
using Application.Helpers;
using Application.Services.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Common;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Application.Services.Implementations
{
    public class WhatsappUserService : IWhatsappUserService
    {
        private readonly IWhatsappUserRepository _waUserRepository;
        private readonly IMapper _mapper;

        public WhatsappUserService(IWhatsappUserRepository waUserRepository,
            IMapper mapper
          )
        {
            _waUserRepository = waUserRepository;
            _mapper = mapper;
        }

        #region CRUD Service Methods
        public async Task<SuccessResponse<WhatsappUserDto>> UpsertWhatsappUser(UpsertWhatsappUserDto model)
        {
            if (model is null)
                throw new RestException(HttpStatusCode.BadRequest, "Validation for whatsapp user");
         
            var isWhatsappUserExist = await _waUserRepository.ExistsAsync(x => x.WaId == model.WaId);

            var waUser = new WhatsappUser();

            //if exist update the lastmessage time
            if (isWhatsappUserExist)
            {
                waUser = await  _waUserRepository.FirstOrDefault(x => x.WaId == model.WaId);
                waUser.LastMessageTime = DateTime.UtcNow;
                var existingWaUserMap = _mapper.Map<WhatsappUser>(waUser);
                _waUserRepository.Update(existingWaUserMap);
            }
            //Create use if not exist
            else
            {
                waUser = _mapper.Map<WhatsappUser>(model);
                await _waUserRepository.AddAsync(waUser);
            }
            
            //save to whatsapp-user
            await _waUserRepository.SaveChangesAsync();


            var response = _mapper.Map<WhatsappUserDto>(waUser);
            return new SuccessResponse<WhatsappUserDto>
            {
                Message = ResponseMessages.Successful,
                Data = response
            };
        }

        public async Task<SuccessResponse<bool>> DeleteWhatsappUserById(string waId)
        {
            var waUser = await _waUserRepository.FirstOrDefault(x => x.WaId == waId);
            if(waUser == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.UserNotFound);

            _waUserRepository.Remove(waUser);
            await _waUserRepository.SaveChangesAsync();

            return new SuccessResponse<bool>
            {
                Message = ResponseMessages.Successful
            };
        }
        #endregion

        #region Query Methods
        public async Task<SuccessResponse<WhatsappUserDto>> GetWhatsappUserByWaId(string waId)
        {
            var waUser = await _waUserRepository.FirstOrDefault(x => x.WaId == waId);

            if (waUser == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.UserNotFound);

            var response = _mapper.Map<WhatsappUserDto>(waUser);
            return new SuccessResponse<WhatsappUserDto>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = response
            };
        }

        public async Task<PagedResponse<IEnumerable<WhatsappUserDto>>> GetWhatsappUsers(ResourceParameter parameter, string name, IUrlHelper urlHelper)
        {
            var waUserQuery = _waUserRepository.GetWhatsappUsersQuery(parameter.Search);
            var waUserResponses = waUserQuery.ProjectTo<WhatsappUserDto>(_mapper.ConfigurationProvider);

            var waUsers = await PagedList<WhatsappUserDto>.CreateAsync(waUserResponses, parameter.PageNumber, parameter.PageSize, parameter.Sort);
            var page = PageUtility<WhatsappUserDto>.CreateResourcePageUrl(parameter, name, waUsers, urlHelper);

            var response = new PagedResponse<IEnumerable<WhatsappUserDto>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = waUsers,
                Meta = new Helpers.Meta
                {
                    Pagination = page
                }
            };
            return response;
        }
        #endregion
    }
}
