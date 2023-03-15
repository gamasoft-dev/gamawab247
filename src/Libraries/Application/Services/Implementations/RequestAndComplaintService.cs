using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.DTOs.PartnerContentDtos;
using Application.Helpers;
using Application.Services.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Common;
using Domain.Entities;
using Domain.Entities.RequestAndComplaints;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Meta = Application.Helpers.Meta;

namespace Application.Services.Implementations
{
    public class RequestAndComplaintService : IRequestAndComplaintService
    {
        private readonly IRepository<RequestAndComplaint> _requestAndComplaintRepo;
        private readonly IMapper _mapper;

        public RequestAndComplaintService(IRepository<RequestAndComplaint> requestAndComplaintRepo, IMapper mapper)
        {
            _requestAndComplaintRepo = requestAndComplaintRepo;
            _mapper = mapper;
        }

        public async Task<SuccessResponse<RequestAndComplaintDto>> CreateRequestAndComplaint(CreateRequestAndComplaintDto model)
        {
            if (string.IsNullOrEmpty(model.Detail))
                throw new RestException(System.Net.HttpStatusCode.BadRequest, "Detail cannot be null: Please provide a detailed description for your request od complaint");

            RequestAndComplaint requestOrComplaint = _mapper.Map<RequestAndComplaint>(model);
            requestOrComplaint.TicketId = requestOrComplaint.GenerateTicketId();

            await _requestAndComplaintRepo.AddAsync(requestOrComplaint);
            await _requestAndComplaintRepo.SaveChangesAsync();

            RequestAndComplaintDto requestAndComplaintDto = _mapper.Map<RequestAndComplaintDto>(requestOrComplaint);
            return new SuccessResponse<RequestAndComplaintDto>
            {
                Data = requestAndComplaintDto,
                Message = "request submitted successfully"
            };
        }

        public async Task<SuccessResponse<bool>> DeleteRequestAndComplaint(Guid id)
        {
            if (id == Guid.Empty)
                throw new RestException(System.Net.HttpStatusCode.BadRequest, "id cannot be null");

            var requestOrComplaint = await _requestAndComplaintRepo.GetByIdAsync(id)
                ?? throw new RestException(System.Net.HttpStatusCode.NotFound, "Record not found for the id provided");

            _requestAndComplaintRepo.Remove(requestOrComplaint);

            await _requestAndComplaintRepo.SaveChangesAsync();

            return new SuccessResponse<bool>
            {
                Message = ResponseMessages.Successful
            };
        }


        public async Task<PagedResponse<IEnumerable<RequestAndComplaintDto>>> GetAllRequestAndComplaint(ResourceParameter parameter, string endPointName, IUrlHelper url)
        {
            var queryable = _requestAndComplaintRepo
                  .Query(x => string.IsNullOrEmpty(parameter.Search)
                              || x.Subject.ToLower().Contains(parameter.Search.ToLower()) || 
                              x.ResolutionStatus.ToLower().Contains(parameter.Search.ToLower()));

            var queryProjection = queryable.ProjectTo<RequestAndComplaintDto>(_mapper.ConfigurationProvider);

            var partners = await PagedList<RequestAndComplaintDto>.CreateAsync(queryProjection, parameter.PageNumber, parameter.PageSize, parameter.Sort);
            var page = PageUtility<RequestAndComplaintDto>.CreateResourcePageUrl(parameter, endPointName, partners, url);

            var response = new PagedResponse<IEnumerable<RequestAndComplaintDto>>
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

        public async Task<PagedResponse<IEnumerable<RequestAndComplaintDto>>> GetAllRequestAndComplaintByBusinessId(Guid businessId, ResourceParameter parameter, string endPointName, IUrlHelper url)
        {
            var queryable = _requestAndComplaintRepo
                  .Query(x => string.IsNullOrEmpty(parameter.Search)
                              || x.Subject.ToLower().Contains(parameter.Search.ToLower()) && x.BusinessId == businessId);
            if (queryable == null)
                throw new RestException(System.Net.HttpStatusCode.BadRequest, ResponseMessages.Failed);

            var queryProjection = queryable.ProjectTo<RequestAndComplaintDto>(_mapper.ConfigurationProvider);

            var partners = await PagedList<RequestAndComplaintDto>.CreateAsync(queryProjection, parameter.PageNumber, parameter.PageSize, parameter.Sort);
            var page = PageUtility<RequestAndComplaintDto>.CreateResourcePageUrl(parameter, endPointName, partners, url);

            var response = new PagedResponse<IEnumerable<RequestAndComplaintDto>>
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

        public async Task<SuccessResponse<RequestAndComplaintDto>> GetRequestAndComplaint(Guid id)
        {
            if (id == Guid.Empty)
                throw new RestException(System.Net.HttpStatusCode.BadRequest, "id cannot be null");

            RequestAndComplaint requestOrComplaint = await _requestAndComplaintRepo.GetByIdAsync(id);
            if (requestOrComplaint == null)
                return new SuccessResponse<RequestAndComplaintDto>
                {
                    Message = "No record was found for the id provided"
                };

            RequestAndComplaintDto requestAndComplaintDtDto = _mapper.Map<RequestAndComplaintDto>(requestOrComplaint);
            return new SuccessResponse<RequestAndComplaintDto>
            {
                Data = requestAndComplaintDtDto,
                Message = ResponseMessages.Successful
            };
        }

        public async Task<SuccessResponse<RequestAndComplaintDto>> GetRequestAndComplaintByTicketId(string ticketId)
        {
            if (string.IsNullOrEmpty(ticketId))
                throw new RestException(System.Net.HttpStatusCode.BadRequest, "Please enter a valid ticket number");

            RequestAndComplaint requestOrComplaint = await _requestAndComplaintRepo.FirstOrDefault(x => x.TicketId == ticketId);
            if (requestOrComplaint == null)
                return new SuccessResponse<RequestAndComplaintDto>
                {
                    Message = "No record was found for the id provided"
                };

            RequestAndComplaintDto requestAndComplaintDtDto = _mapper.Map<RequestAndComplaintDto>(requestOrComplaint);
            return new SuccessResponse<RequestAndComplaintDto>
            {
                Data = requestAndComplaintDtDto,
                Message = ResponseMessages.Successful
            };
        }

        public async Task<SuccessResponse<RequestAndComplaintDto>> UpdateRequestAndComplaint(Guid id, UpdateRequestAndComplaintDto model)
        {
            if (id == Guid.Empty || model == null)
                throw new RestException(System.Net.HttpStatusCode.BadRequest, ResponseMessages.Failed);

            var requestORComplaint = await _requestAndComplaintRepo.GetByIdAsync(id)
                ?? throw new RestException(System.Net.HttpStatusCode.NotFound, ResponseMessages.Failed);

            requestORComplaint.ResolutionStatus = model.ResolutionStatus;
            requestORComplaint.ResolutionDate = DateTime.UtcNow;
            requestORComplaint.Responses = model.Response;
            requestORComplaint.TicketId = model.TicketId;
            requestORComplaint.CallBackUrl = model.CallBackUrl;
            requestORComplaint.Detail = model.Detail;

            _requestAndComplaintRepo.Update(requestORComplaint);
            await _requestAndComplaintRepo.SaveChangesAsync();

            RequestAndComplaintDto requestAndComplainResponse = _mapper.Map<RequestAndComplaintDto>(requestORComplaint);
            return new SuccessResponse<RequestAndComplaintDto>
            {
                Data = requestAndComplainResponse,
                Message = ResponseMessages.Successful
            };
        }
    }
}
