using Application.DTOs.BusinessDtos;
using Application.Helpers;
using Application.Services.Interfaces;
using Application.Services.Interfaces.FormProcessing;
using AutoMapper;
using Domain.Common;
using Domain.Entities.DialogMessageEntitties;
using Domain.Entities.FormProcessing;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace Application.Services.Implementations.FormProcessing
{
    public class BusinessFormService : IBusinessFormService
    {
        private readonly IRepository<BusinessForm> _businessFormRepo;
        private readonly IMapper _mapper;
        private readonly IBusinessService _businessService;
        private readonly IRepository<BusinessMessage> _businessMessageRepo;

        public BusinessFormService(IRepository<BusinessForm> businessFormRepo, IMapper mapper,
            IRepository<BusinessMessage> businessMessageRepo, IBusinessService businessService)
        {
            _businessFormRepo = businessFormRepo;
            _mapper = mapper;
            _businessMessageRepo = businessMessageRepo;
            _businessService = businessService;
        }

        public async Task<SuccessResponse<BusinessFormDto>> CreateBusinessForm(CreateBusinessFormDto model)
        {
            if (model is null)
                throw new RestException(HttpStatusCode.BadRequest, "One/More parameter could not be validated");

            if (model.BusinessConversationId == Guid.Empty || model.BusinessId == Guid.Empty)
                throw new RestException(HttpStatusCode.BadRequest, "Business Id or Conversation Id failed to validate");

            //Check if business exist.
            var business = await _businessService.GetBusinessByBusinessId(model.BusinessId);
            if (business is null)
                throw new RestException(HttpStatusCode.NotFound, "Error: No business exist with id exist");

            //check if convo exists
            var conversation = await _businessMessageRepo.FirstOrDefault(x => x.Id == model.BusinessConversationId);
            if (conversation is null)
                throw new RestException(HttpStatusCode.NotFound, "Error: No business conversation exist with id exist");

            var businessFormMap = _mapper.Map<BusinessForm>(model);

            await _businessFormRepo.AddAsync(businessFormMap);
            await _businessFormRepo.SaveChangesAsync();
            
            var response = _mapper.Map<BusinessFormDto>(businessFormMap);

            return new SuccessResponse<BusinessFormDto>
            {
                Data = response,
                Message = "Successfully processed",
                Success = true
            };
        }

        public async Task UpdateBusinessForm(Guid id, UpdateBusinessFormDto model)
        {
            if (id == Guid.Empty || model is null)
                throw new RestException(HttpStatusCode.BadRequest, "Failed to validate parameter");

            var get = await _businessFormRepo.FirstOrDefault(x => x.Id == id);
            if (get is null)
                throw new RestException(HttpStatusCode.NotFound, "Error: No business form exist with id exist");


            var businessFormMap = _mapper.Map<BusinessForm>(model);

            for (int i = 1; i < businessFormMap.FormElements.Count; i++)//to auto handle form indexes accordingly
            {
                businessFormMap.FormElements[i].Id = i;
            }

            get.UpdatedAt = DateTime.UtcNow;
            get.Headers = businessFormMap.Headers;
            get.ResponseKvps = businessFormMap.ResponseKvps;
            get.FormElements = businessFormMap.FormElements;
            get.UrlMethodType = businessFormMap.UrlMethodType;
            get.SubmissionUrl = businessFormMap.SubmissionUrl;

            _businessFormRepo.Update(get);
            await _businessFormRepo.SaveChangesAsync();
        }

        public async Task<SuccessResponse<BusinessFormDto>> GetBusinessFormById(Guid id)
        {
            if (id == Guid.Empty)
                throw new RestException(HttpStatusCode.BadRequest, "Failed to validate parameter");

            var get = await _businessFormRepo.FirstOrDefault(x => x.Id == id);
            if (get is null)
                throw new RestException(HttpStatusCode.NotFound, "Error: No business form exist with id exist");

            var response = _mapper.Map<BusinessFormDto>(get);

            return new SuccessResponse<BusinessFormDto>
            {
                Data = response,
                Success = true,
                Message = "Successfully retrieved"
            };
        }

        public async Task<SuccessResponse<BusinessForm>> GetBusinessFormByBusinessId(Guid businessId)
        {
            if (businessId == Guid.Empty)
                throw new RestException(HttpStatusCode.BadRequest, "Failed to validate parameter");

            var get = await _businessFormRepo.FirstOrDefault(x => x.BusinessId == businessId);
            if (get is null)
                throw new RestException(HttpStatusCode.NotFound, "Error: No business form " +
                    "exist with this specified business-id exist");

            return new SuccessResponse<BusinessForm>
            {
                Data = get,
                Success = true,
                Message = "Successfully retrieved"
            };
        }

        public async Task<PagedResponse<IEnumerable<BusinessForm>>> GetAllBusinessFormByUserId(string search,
            string name, ResourceParameter parameter, IUrlHelper urlHelper)
        {
            var query = await _businessFormRepo.FindAsync(x => x.CreatedById == WebHelper.UserId &&
                                (string.IsNullOrWhiteSpace(search)
                                || x.SubmissionUrl.Trim().ToLower().Contains(search.Trim().ToLower())
                                || x.BusinessConversationId.ToString() == search
                                || x.BusinessId.ToString() == search
                                || x.Id.ToString() == search) &&
                                (!parameter.StartDate.HasValue || !parameter.EndDate.HasValue
                                || (x.CreatedAt >= parameter.StartDate && x.CreatedAt <= parameter.EndDate)
                                || (x.UpdatedAt >= parameter.StartDate && x.UpdatedAt <= parameter.EndDate)));

            var pagedList = await PagedList<BusinessForm>.CreateAsync
                (query.AsQueryable(), parameter.PageNumber, parameter.PageSize, parameter.Sort);

            var page = PageUtility<BusinessForm>.CreateResourcePageUrl(parameter, name, pagedList, urlHelper);

            return new PagedResponse<IEnumerable<BusinessForm>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = pagedList,
                Meta = new Helpers.Meta
                {
                    Pagination = page
                }
            };
        }

        public async Task<BusinessForm> GetBusinessFormFisrtOrDefault(Expression<Func<BusinessForm, bool>> expression)
        {
            var iquery = _businessFormRepo.Query(expression).Include(x => x.Business);
            return await iquery.FirstOrDefaultAsync();
        }

        public async Task Update(BusinessForm formElement)
        {
            var model = _businessFormRepo.GetByIdAsync(formElement.Id);
            if (model is null)
                throw new ArgumentNullException(nameof(formElement));

            _businessFormRepo.Update(formElement);
            await _businessFormRepo.SaveChangesAsync();
        }

        //public Task<BusinessForm> Create(BusinessForm formElement)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
