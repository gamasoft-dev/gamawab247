using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.PartnerContentDtos;
using Application.DTOs.RequestAndComplaintDtos;
using Application.Helpers;
using Application.Services.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Common;
using Domain.Entities;
using Domain.Entities.RequestAndComplaints;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Application.Services.Implementations
{
    public class RequestAndComplaintConfigService : IRequestAndComplaintConfigService
    {
        private readonly IRepository<RequestAndComplaintConfig> _requestAndComplaintConfigRepo;
        private readonly IRepository<Business> _businessRepo;
        private readonly IMapper _mapper;
        public RequestAndComplaintConfigService(IRepository<RequestAndComplaintConfig> requestAndComplaintRepo, IMapper mapper, IRepository<Business> businessRepo)
        {
            _requestAndComplaintConfigRepo = requestAndComplaintRepo;
            _mapper = mapper;
            _businessRepo = businessRepo;
        }

        public async Task<SuccessResponse<RequestAndComplaintConfigDto>> CreateRequestAndComplaintConfig(CreateRequestAndComplaintConfigDto model)
        {
            if (model == null)
                throw new RestException(System.Net.HttpStatusCode.BadRequest, "Detail cannot be null: Please provide a detailed description for your request od complaint");

            //confirm the business exist
            var business = _businessRepo.Exists(x => x.Id == model.BusinessId);
            if (!business)
                throw new RestException(System.Net.HttpStatusCode.NotFound, "Invalid request: no business found for the provided business id");
            
            RequestAndComplaintConfig requestOrComplaintConfig = _mapper.Map<RequestAndComplaintConfig>(model);

            await _requestAndComplaintConfigRepo.AddAsync(requestOrComplaintConfig);
            await _requestAndComplaintConfigRepo.SaveChangesAsync();

            RequestAndComplaintConfigDto requestAndComplaintConfigDto = _mapper.Map<RequestAndComplaintConfigDto>(requestOrComplaintConfig);
            return new SuccessResponse<RequestAndComplaintConfigDto>
            {
                Data = requestAndComplaintConfigDto,
                Message = "request submitted successfully"
            };
        }

        public async Task<SuccessResponse<bool>> DeleteRequestAndComplaintConfig(Guid id)
        {
            if (id == Guid.Empty)
                throw new RestException(System.Net.HttpStatusCode.BadRequest, "id cannot be null");

            var requestOrComplaintConfig = await _requestAndComplaintConfigRepo.GetByIdAsync(id)
                ?? throw new RestException(System.Net.HttpStatusCode.NotFound, "Record not found for the id provided");

            _requestAndComplaintConfigRepo.Remove(requestOrComplaintConfig);

            await _requestAndComplaintConfigRepo.SaveChangesAsync();

            return new SuccessResponse<bool>
            {
                Message = ResponseMessages.Successful
            };
        }

        public async Task<PagedResponse<IEnumerable<RequestAndComplaintConfigDto>>> GetAllRequestAndComplaintConfig(ResourceParameter parameter, string endPointName, IUrlHelper url)
        {
            var queryable = _requestAndComplaintConfigRepo
                  .Query(x => string.IsNullOrEmpty(parameter.Search)
                              || x.PartnerContentProcessorKey.ToLower().Contains(parameter.Search.ToLower()));

            var queryProjection = queryable.ProjectTo<RequestAndComplaintConfigDto>(_mapper.ConfigurationProvider);

            var partners = await PagedList<RequestAndComplaintConfigDto>.CreateAsync(queryProjection, parameter.PageNumber, parameter.PageSize, parameter.Sort);
            var page = PageUtility<RequestAndComplaintConfigDto>.CreateResourcePageUrl(parameter, endPointName, partners, url);

            var response = new PagedResponse<IEnumerable<RequestAndComplaintConfigDto>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = partners,
                Meta = new Meta
                {
                    Pagination = page
                }
            };
            return response;
        }

        public async Task<PagedResponse<IEnumerable<RequestAndComplaintConfigDto>>> GetAllRequestAndComplaintConfigByBusinessId(Guid businessId, ResourceParameter parameter, string endPointName, IUrlHelper url)
        {
            var queryable = _requestAndComplaintConfigRepo
                 .Query(x => string.IsNullOrEmpty(parameter.Search)
                             || x.BusinessId == businessId) ?? throw new RestException(System.Net.HttpStatusCode.BadRequest, ResponseMessages.Failed);
            
            var queryProjection = queryable.ProjectTo<RequestAndComplaintConfigDto>(_mapper.ConfigurationProvider);

            var partners = await PagedList<RequestAndComplaintConfigDto>.CreateAsync(queryProjection, parameter.PageNumber, parameter.PageSize, parameter.Sort);
            var page = PageUtility<RequestAndComplaintConfigDto>.CreateResourcePageUrl(parameter, endPointName, partners, url);

            var response = new PagedResponse<IEnumerable<RequestAndComplaintConfigDto>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = partners,
                Meta = new Meta
                {
                    Pagination = page
                }
            };
            return response;

        }

        public async Task<SuccessResponse<RequestAndComplaintConfigDto>> GetRequestAndComplaintConfig(Guid id)
        {
            if (id == Guid.Empty)
                throw new RestException(System.Net.HttpStatusCode.BadRequest, "id cannot be null");

            RequestAndComplaintConfig requestOrComplaint = await _requestAndComplaintConfigRepo.GetByIdAsync(id);
            if (requestOrComplaint == null)
                return new SuccessResponse<RequestAndComplaintConfigDto>
                {
                    Message = "No record was found for the id provided"
                };

            RequestAndComplaintConfigDto requestAndComplaintDtDto = _mapper.Map<RequestAndComplaintConfigDto>(requestOrComplaint);
            return new SuccessResponse<RequestAndComplaintConfigDto>
            {
                Data = requestAndComplaintDtDto,
                Message = ResponseMessages.Successful
            };
        }

        public async Task<SuccessResponse<RequestAndComplaintConfigDto>> UpdateRequestAndComplaintConfig(Guid id, UpdateRequestAndComplaintConfigDto model)
        {
            if (id == Guid.Empty || model == null)
                throw new RestException(System.Net.HttpStatusCode.BadRequest, ResponseMessages.Failed);

            var requestORComplaintConfig = await _requestAndComplaintConfigRepo.GetByIdAsync(id)
                ?? throw new RestException(System.Net.HttpStatusCode.NotFound, ResponseMessages.Failed);

            var updateRequestOrComplaintConfigMap = _mapper.Map(model, requestORComplaintConfig);
           
            _requestAndComplaintConfigRepo.Update(updateRequestOrComplaintConfigMap);
            await _requestAndComplaintConfigRepo.SaveChangesAsync();

            RequestAndComplaintConfigDto requestAndComplaintConfigResponse = _mapper.Map<RequestAndComplaintConfigDto>(requestORComplaintConfig);
            return new SuccessResponse<RequestAndComplaintConfigDto>
            {
                Data = requestAndComplaintConfigResponse,
                Message = ResponseMessages.Successful
            };
        }
    }
}
