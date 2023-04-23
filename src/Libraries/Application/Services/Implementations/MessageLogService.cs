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
    public class MessageLogService : IMessageLogService
    {
        private readonly IMessageLogRepository _messageLogRepository;
        private readonly IMapper _mapper;

        public MessageLogService(IMessageLogRepository messageLogRepository, IMapper mapper)
        {
            _mapper = mapper;
            _messageLogRepository = messageLogRepository;
        }


        #region CRUD methods
        public async Task<SuccessResponse<MessageLogDto>> CreateMessageLog(CreateMessageLogDto model)
        {
            if (model is null)
                throw new RestException(HttpStatusCode.BadRequest, "Validation for message log");

            var messageLog = _mapper.Map<MessageLog>(model);

            await _messageLogRepository.AddAsync(messageLog);
            await _messageLogRepository.SaveChangesAsync();

            var response = _mapper.Map<MessageLogDto>(messageLog);

            return new SuccessResponse<MessageLogDto>
            {
                Message = ResponseMessages.CreationSuccessResponse,
                Data = response
            };
        }

        public async Task<SuccessResponse<MessageLogDto>> UpdateMessageLog(UpdateMessageLogDto model)
        {
            if (model is null)
                throw new RestException(HttpStatusCode.BadRequest, "Validation for whatsapp user");

            var newMessageLog = _mapper.Map<MessageLog>(model);
            _messageLogRepository.Update(newMessageLog);
            await _messageLogRepository.SaveChangesAsync();

            var response = _mapper.Map<MessageLogDto>(newMessageLog);

            return new SuccessResponse<MessageLogDto>
            {
                Message = ResponseMessages.CreationSuccessResponse,
                Data = response
            };
        }

        public async Task<SuccessResponse<bool>> DeleteMessageLogById(Guid id)
        {
            var messageLog = await _messageLogRepository.FirstOrDefault(x => x.Id == id);
            if (messageLog == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.LogNotFound);

            _messageLogRepository.Remove(messageLog);
            await _messageLogRepository.SaveChangesAsync();

            return new SuccessResponse<bool>
            {
                Message = ResponseMessages.Successful
            };
        }
        #endregion

        #region Query Methods
        public async Task<SuccessResponse<MessageLogDto>> GetMessageLogById(Guid id)
        {
            var messageLog = await _messageLogRepository.FirstOrDefault(x => x.Id == id);
            if (messageLog == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.LogNotFound);

            var response = _mapper.Map<MessageLogDto>(messageLog);
            return new SuccessResponse<MessageLogDto>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = response
            };
        }

        public async Task<PagedResponse<IEnumerable<MessageLogDto>>> GetMessageLogs(ResourceParameter parameter, string name, IUrlHelper urlHelper)
        {
            var messageLogQuery = _messageLogRepository.GetMessageLogQuery(parameter?.Search);
            var messageLogResponses = messageLogQuery.ProjectTo<MessageLogDto>(_mapper.ConfigurationProvider);

            var messageLogs = await PagedList<MessageLogDto>.CreateAsync(messageLogResponses, parameter.PageNumber, parameter.PageSize, parameter.Sort);
            var page = PageUtility<MessageLogDto>.CreateResourcePageUrl(parameter, name, messageLogs, urlHelper);

            var response = new PagedResponse<IEnumerable<MessageLogDto>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = messageLogs,
                Meta = new Helpers.Meta
                {
                    Pagination = page
                }
            };
            return response;
        }

        public async Task<PagedResponse<IEnumerable<MessageLogDto>>> GetMessageLogsByWaId(Guid waId, ResourceParameter parameter, string name, IUrlHelper urlHelper)
        {
            var messageLogQuery = _messageLogRepository.CreateMessageLogQuerable(waId, parameter.Search);

            var messageLogResponses = messageLogQuery.ProjectTo<MessageLogDto>(_mapper.ConfigurationProvider);

            var messageLogs = await PagedList<MessageLogDto>.CreateAsync(messageLogResponses, parameter.PageNumber, parameter.PageSize, parameter.Sort);
            var page = PageUtility<MessageLogDto>.CreateResourcePageUrl(parameter, name, messageLogs, urlHelper);

            var response = new PagedResponse<IEnumerable<MessageLogDto>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = messageLogs,
                Meta = new Helpers.Meta
                {
                    Pagination = page
                }
            };
            return response;
        }

        public async Task<PagedResponse<IEnumerable<MessageLogDto>>> GetMessageLogsByPhoneNumber(string waId, ResourceParameter parameter, string name, IUrlHelper urlHelper)
        {
            var processedWaId = PreprocessPhoneNumber(waId);
            
            var messageLogQuery = _messageLogRepository.CreateMessageLogQuerable(processedWaId, parameter.Search);

            var messageLogResponses = messageLogQuery.ProjectTo<MessageLogDto>(_mapper.ConfigurationProvider);

            var messageLogs = await PagedList<MessageLogDto>.CreateAsync(messageLogResponses, parameter.PageNumber, parameter.PageSize, parameter.Sort);
            var page = PageUtility<MessageLogDto>.CreateResourcePageUrl(parameter, name, messageLogs, urlHelper);

            var response = new PagedResponse<IEnumerable<MessageLogDto>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = messageLogs,
                Meta = new Helpers.Meta
                {
                    Pagination = page
                }
            };
            return response;
        }
        #endregion

        private static string PreprocessPhoneNumber(string phoneNumber, string countryCode = "234")
        {
            if (phoneNumber.StartsWith("0"))
            {
                phoneNumber = phoneNumber[1..]; // remove the leading zero
            }

            if (!phoneNumber.StartsWith(countryCode))
            {
                phoneNumber = countryCode + phoneNumber; // append the country code if not provided
            }

            return phoneNumber;
        }
    }
}
