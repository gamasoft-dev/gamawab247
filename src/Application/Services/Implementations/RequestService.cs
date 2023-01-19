using Application.DTOs;
using Application.Helpers;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Application.Services.Interface;
using System.Threading.Tasks;

namespace Application.Services.Implementations
{
    public class RequestService : IRequestService
    {
        private readonly IRepository<Request> _requestRepository;
        private readonly IRepository<RequestOption> _requestOptionRepository;
        private readonly ILogger<RequestService> _logger;
        private readonly IMapper _mapper;

        public RequestService(IRepository<RequestOption> requestOptionRepository,
            IRepository<Request> requestRepository,
            ILogger<RequestService> logger, IMapper mapper)
        {
            _requestOptionRepository = requestOptionRepository;
            _requestRepository = requestRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<RequestDto> CreateRequestConversations(RequestDto requestDto)
        {
            try
            {
                if (requestDto is null || requestDto.BusinessId == Guid.Empty || !requestDto.RequestOption.Any())
                {
                    throw new RestException(HttpStatusCode.BadRequest, "One or more validation error.");
                }

                var request = _mapper.Map<Request>(requestDto);
                var resp = await _requestRepository.AddAndReturnValue(request);
                if (resp != null)
                {
                    foreach (var option in requestDto.RequestOption)
                    {
                        //check if request keyword already exist for this business
                        var checkForExistingkeyWord = await CheckRequestOptionKeyWordExistenceForBusiness(requestDto.BusinessId,
                        requestDto.RequestOption.FirstOrDefault().RequestKeyword);

                        if (checkForExistingkeyWord)
                        {
                            option.RequestId = resp.Id;
                            var requestOption = _mapper.Map<RequestOption>(option);
                            await _requestOptionRepository.AddAsync(option);
                            await _requestOptionRepository.SaveChangesAsync();
                        }
                    }
                    return requestDto;
                }
                else
                    throw new RestException(HttpStatusCode.InternalServerError, $"An error occurred.");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                throw;
            }
        }

        public async Task<IEnumerable<GetRequestDto>> GetAllByBusinessId(Guid? businessId)
        {
            try
            {
                if (businessId == Guid.Empty)
                {
                    throw new RestException(HttpStatusCode.BadRequest, $"One or more validation error.");
                }

                var requestByBusinessId = _requestRepository.Query(x => x.BusinessId == businessId).AsEnumerable();

                return await Task.FromResult(_mapper.Map<IEnumerable<GetRequestDto>>(requestByBusinessId));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                throw;
            }
        }

        public async Task<IEnumerable<GetRequestDto>> GetAllByIndustryId(Guid? industryId)
        {
            try
            {
                if (industryId == Guid.Empty)
                {
                    throw new RestException(HttpStatusCode.BadRequest, $"One or more validation error.");
                }

                var requestByIndustryId = _requestRepository.Query(x => x.IndustryId == industryId).AsEnumerable();
                return await Task.FromResult(_mapper.Map<IEnumerable<GetRequestDto>>(requestByIndustryId));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                throw;
            }
        }

        public async Task<GetRequestDto> GetByRequestId(Guid requestId)
        {
            try
            {
                if (requestId == Guid.Empty)
                {
                    throw new RestException(HttpStatusCode.BadRequest, $"One or more validation error.");
                }
                var requests = await _requestRepository.FirstOrDefault(x => x.Id == requestId);
                var resp = _mapper.Map<GetRequestDto>(requests);
                if (requests is null)
                {
                    return new GetRequestDto();
                }
                return resp;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                throw;
            }
        }

        public async Task<GetRequestDto> GetAllRequestOptionsByRequestId(Guid requestId)
        {
            try
            {
                if (requestId == Guid.Empty)
                {
                    throw new RestException(HttpStatusCode.BadRequest, $"One or more validation error.");
                }

                var request = await GetByRequestId(requestId);
                if (request is null)
                {
                    return new GetRequestDto();
                }
                var requestOptions = _requestOptionRepository.Query(x => x.RequestId == request.Id);
                request.RequestOption = (List<RequestOption>)requestOptions;
                return request;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                throw;
            }
        }

        private async Task<bool> CheckRequestOptionKeyWordExistenceForBusiness(Guid? businessId, string keyWord)
        {
            try
            {
                if (businessId == Guid.Empty && string.IsNullOrEmpty(keyWord))
                {
                    throw new RestException(HttpStatusCode.BadRequest, $"One or more validation error.");
                }

                var request = await this.GetAllByBusinessId(businessId);
                foreach (var item in request)
                {
                    var requestOptions = _requestOptionRepository.Query(x => x.RequestId == item.Id);
                    foreach (var requestOption in requestOptions)
                    {
                        if (requestOption.RequestKeyword.Trim().ToLower().Equals(keyWord.Trim().ToLower()))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                throw;
            }
        }
    }
}
