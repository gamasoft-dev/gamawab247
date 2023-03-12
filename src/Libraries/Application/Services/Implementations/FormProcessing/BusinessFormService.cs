using Application.DTOs.BusinessDtos;
using Application.Helpers;
using Application.Services.Interfaces;
using Application.Services.Interfaces.FormProcessing;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Common;
using Domain.Entities.DialogMessageEntitties;
using Domain.Entities.FormProcessing;
using Domain.Entities.FormProcessing.ValueObjects;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Org.BouncyCastle.Math.EC.Rfc7748;
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

            //Check if business exist.
            var business = await _businessService.GetBusinessByBusinessId(model.BusinessId);

            if (business is null)
                throw new RestException(HttpStatusCode.NotFound, "Error: No business exist with id exist");

            // validate form
            await ValidateBusinessForm(model);


            var businessForm = _mapper.Map<BusinessForm>(model);

            await _businessFormRepo.AddAsync(businessForm);
            await _businessFormRepo.SaveChangesAsync();
            
            var businessFormDto = _mapper.Map<BusinessFormDto>(businessForm);

            return new SuccessResponse<BusinessFormDto>
            {
                Data = businessFormDto,
                Message = "Successfully processed",
                Success = true
            };
        }

        public async Task<SuccessResponse<bool>> UpdateBusinessForm(Guid id, UpdateBusinessFormDto model)
        {
            if (model is null)
                throw new RestException(HttpStatusCode.BadRequest, "model is null");

            model.Id = id;
            var businessForm = await _businessFormRepo.FirstOrDefault(x => x.Id == id);
            if (businessForm is null)
                throw new RestException(HttpStatusCode.NotFound, "Error: No business form exist with this id");

            // validate form
            await ValidateBusinessForm(model);

            var businessFormMap = _mapper.Map(model, businessForm);

            businessForm.UpdatedAt = DateTime.UtcNow;

            await _businessFormRepo.SaveChangesAsync();

            return new SuccessResponse<bool>
            {
                Data = true,
                Message = ResponseMessages.UpdateResponse
            };
        }

        public async Task<SuccessResponse<BusinessFormDto>> GetBusinessFormById(Guid id)
        {
            if (id == Guid.Empty)
                throw new RestException(HttpStatusCode.BadRequest, "Failed to validate parameter");

            var businessForm = await _businessFormRepo.FirstOrDefault(x => x.Id == id);
            if (businessForm is null)
                throw new RestException(HttpStatusCode.NotFound, "Error: No business form exist with id exist");

            var response = _mapper.Map<BusinessFormDto>(businessForm);

            return new SuccessResponse<BusinessFormDto>
            {
                Data = response,
                Success = true,
                Message = "Successfully retrieved"
            };
        }

        public async Task<PagedResponse<IEnumerable<BusinessFormDto>>> GetAllByBusinessId(Guid businessId, string search,
            string name, ResourceParameter parameter, IUrlHelper urlHelper)
        {
            var query =  _businessFormRepo.Query(x => x.BusinessId == businessId &&
                                (string.IsNullOrWhiteSpace(search)
                                || x.SubmissionUrl.Trim().ToLower().Contains(search.Trim().ToLower())
                                || x.BusinessConversationId.ToString() == search
                                || x.BusinessId.ToString() == search
                                || x.Id.ToString() == search) &&
                                (!parameter.StartDate.HasValue || !parameter.EndDate.HasValue
                                || (x.CreatedAt >= parameter.StartDate && x.CreatedAt <= parameter.EndDate)
                                || (x.UpdatedAt >= parameter.StartDate && x.UpdatedAt <= parameter.EndDate)));

            var queryProjection = query.ProjectTo<BusinessFormDto>(_mapper.ConfigurationProvider);

            var pagedList = await PagedList<BusinessFormDto>.CreateAsync(queryProjection, parameter.PageNumber, parameter.PageSize, parameter.Sort);
            

            var page = PageUtility<BusinessFormDto>.CreateResourcePageUrl(parameter, name, pagedList, urlHelper);
           
            return new PagedResponse<IEnumerable<BusinessFormDto>>
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




        private async Task ValidateBusinessForm(CreateBusinessFormDto model)
        {
            if (model.ConclusionBusinessMessageId.HasValue)
            {
                var conclusionBusinessMessage = await _businessMessageRepo
                    .GetByIdAsync(id: model.ConclusionBusinessMessageId.Value);

                if (conclusionBusinessMessage is null)
                    throw new RestException(HttpStatusCode.BadRequest, "Conclusive business messsage doesnt exist. Invalid ConclusionBusinessMessageId");
            }

            List<FormElement> validFormElements = new ();
            HashSet<FormElement> formElements = new HashSet<FormElement>(model.FormElements);

            if (model.FormElements.Count != formElements.Count)
                throw new RestException(HttpStatusCode.BadRequest, "No two form elements should have same Position or key values");

            var lastFormElement = model.FormElements.Where(x => x.IsLastFormElement);

            if (lastFormElement.Count() > 1)
                throw new RestException(HttpStatusCode.BadRequest,"You cannot have more than one last form Element configured");

            // for each of the form elements

            foreach (var formElement in model.FormElements)
            {
                if(formElement.ShouldRetrieveContentExternally &&
                   string.IsNullOrEmpty(formElement.PartnerContentProcessorKey))
                {
                    throw new RestException(HttpStatusCode.BadRequest, $"{formElement.Key} If the ShouldRetrieveContentExternally  is true, " +
                        $"then the PartnerContentProcessorKey cannot be null");
                }

                if (!string.IsNullOrEmpty(formElement.Label) &&
                    (!string.IsNullOrEmpty(formElement.PartnerContentProcessorKey)
                    || formElement.ShouldRetrieveContentExternally))
                {
                    throw new RestException(HttpStatusCode.BadRequest, $"The {formElement.Key} form element has a value for its label property" +
                       $"therefore the ShouldRetrieveContentExternally should be false and PartnerContentProcessorKey should be empty");
                }

                if (formElement.FollowUpMessageId.HasValue)
                {
                    // validate that this is a valid business message
                    var followUpMessage = await _businessMessageRepo
                    .GetByIdAsync(id: formElement.FollowUpMessageId.Value);

                    if (followUpMessage is null)
                        throw new RestException(HttpStatusCode.BadRequest, $"Invalid folllowUp message configured for form element {formElement.Key}. Invalid Id");

                }
                if (!formElement.IsLastFormElement)
                {
                    var nextFormElement = formElements.FirstOrDefault(x => x.Position == formElement.NextFormElementPosition);

                    if (formElement.NextFormElementPosition == 0)
                        throw new RestException(HttpStatusCode.BadRequest, $"NextFormElementPosition value is compulsory for form elements that are not set as lastFormElement and cannot be set to 0");


                    if (nextFormElement is null || nextFormElement.Equals(formElement))
                        throw new RestException(HttpStatusCode.BadRequest, $"Invalid NextFormElementPoistion value for form element: {formElement.Key}");

                }

            }
        }

    }
}
