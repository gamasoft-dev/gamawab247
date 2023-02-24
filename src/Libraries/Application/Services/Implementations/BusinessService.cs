using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Application.DTOs;
using Application.DTOs.BusinessDtos;
using Application.Helpers;
using Application.Services.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Common;
using Domain.Entities;
using Domain.Entities.Identities;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Application.Services.Implementations
{
    public class BusinessService : IBusinessService
    {
        private readonly IMapper _mapper;
        //private readonly ILogger<BusinessService> _logger;
        private readonly IRepository<Business> _businessRepository;
        private readonly IRepository<Industry> _industryRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IIndustryService _industryService;
        private readonly UserManager<User> _userManager;

        public BusinessService(IMapper mapper,
            //ILogger<BusinessService> logger,
            UserManager<User> userManager,
            IRepository<Business> businessRepository, IIndustryService industryService, 
            IRepository<Industry> industryRepository, IRepository<User> userRepository)
        {
            _mapper = mapper;
            //_logger = logger;
            _businessRepository = businessRepository;
            _industryService = industryService;
            _userManager = userManager;
            _industryRepository = industryRepository;
            _userRepository = userRepository;
        }

        public async Task<SuccessResponse<BusinessDto>> ProcessCreateBusiness(CreateBusinessDto businessDto)
        {
            BusinessDto createdBusinessDto = null;
            try
            {
                if(businessDto==null || string.IsNullOrEmpty(businessDto.Name))
                    throw new RestException(HttpStatusCode.BadRequest, "Business payload or business name cannot be ");

                //Check for existing business..
                var getBusiness = await _businessRepository.FirstOrDefault(p=>p.Email == businessDto.BusinessAdminEmail);
                if (getBusiness != null) throw new RestException(HttpStatusCode.BadRequest,
                    "There is already a business with this business email");
                
                getBusiness = await _businessRepository.FirstOrDefault(p=>p.PhoneNumber == businessDto.PhoneNumber);
                if (getBusiness != null) throw new RestException(HttpStatusCode.BadRequest,
                    "There is already a business with this business phone number");
                
                getBusiness = await _businessRepository.FirstOrDefault(p=>p.Name == businessDto.Name);
                if (getBusiness != null) throw new RestException(HttpStatusCode.BadRequest,
                    "There is already a business with this business name");
                
                var industry = await _industryRepository.FirstOrDefault(x=>x.Id == businessDto.IndustryId);
                if(industry is null) 
                    throw new RestException(HttpStatusCode.BadRequest, "No industry exist with this identifier"/*ResponseMessages.Failed*/);

                
                var user = _mapper.Map<User>(businessDto);
                user.UserName = businessDto.BusinessAdminEmail;
                
                var userCreateResult = await _userManager.CreateAsync(user, "Password123@");
                if(!userCreateResult.Succeeded)
                    throw new RestException(HttpStatusCode.BadRequest, userCreateResult.ToString());

                await AddAdminUserRole(user);

                var business = _mapper.Map<Business>(businessDto);
                business.Industry = industry;
                business.CreatedById = WebHelper.UserId;
                business.AdminUserId = user.Id;

                await _businessRepository.AddAsync(business);

                createdBusinessDto = _mapper.Map<BusinessDto>(business);
                createdBusinessDto.AdminFirstName = user?.FirstName;
                createdBusinessDto.AdminSurName = user?.LastName;
                createdBusinessDto.AdminPhoneNumber = user?.PhoneNumber;
                createdBusinessDto.BusinessAdminEmail = user?.Email;
                createdBusinessDto.AdminId = user.Id;

                user.BusinessId = business.Id;

                //update business details to user.

                await _businessRepository.SaveChangesAsync();
                
            }
            catch (Exception e)
            {
                var error = "An error occured on business creation" + e.Message;
               // _logger.LogCritical(error, e);
                throw new RestException(HttpStatusCode.InternalServerError, e.Message);
            }

            return new SuccessResponse<BusinessDto>
            {
                Data = createdBusinessDto
            };
        }

        public async Task<SuccessResponse<BusinessDto>> GetBusinessByBusinessId(Guid businessId)
        {
            if (businessId == Guid.Empty) return null;

            var get = await _businessRepository.FirstOrDefault(p => p.Id == businessId);
            if (get is null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.BusinessNotFound);

            var businessResponse = _mapper.Map<BusinessDto>(get);
            
            return new SuccessResponse<BusinessDto>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = businessResponse
            };
        }

        public async Task<PagedResponse<IEnumerable<BusinessDto>>> GetAllBusinesses(ResourceParameter parameter,
            string endPointName, IUrlHelper url)
        {
            var queryable = _businessRepository
                .Query(x=> (string.IsNullOrEmpty(parameter.Search) 
                            || (x.Name.ToLower().Contains(parameter.Search.ToLower()) || x.Industry.Name.ToLower().Contains(parameter.Search))));
            
            var queryProjection = queryable.ProjectTo<BusinessDto>(_mapper.ConfigurationProvider);

            var businesses = await PagedList<BusinessDto>.CreateAsync(queryProjection, parameter.PageNumber, parameter.PageSize, parameter.Sort);
            var page = PageUtility<BusinessDto>.CreateResourcePageUrl(parameter, endPointName, businesses, url);

            var response = new PagedResponse<IEnumerable<BusinessDto>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = businesses,
                Meta = new Helpers.Meta
                {
                    Pagination = page
                }
            };
            return response;
        }

        public async Task<SuccessResponse<IEnumerable<BusinessDto>>> GetAllBusinessesByUserEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new RestException(HttpStatusCode.BadRequest, "Validation failed for business owner id");

            var business = await Task.FromResult(_businessRepository.Query(x => x.Email == email));
            var businessResponse = _mapper.ProjectTo<BusinessDto>(business);

            return new SuccessResponse<IEnumerable<BusinessDto>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = businessResponse
            };
            
        }

        /// <summary>
        /// TODO : Fix this 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<SuccessResponse<BusinessDto>> UpdateBusinessInfo(Business model)
        {
            if(model==null || model.Id==Guid.Empty)
                throw new RestException(HttpStatusCode.BadRequest, ResponseMessages.Failed);

            var getBusiness = await _businessRepository.GetByIdAsync(model.Id);
            if (getBusiness == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.Failed);

            getBusiness.AvatarUrl = model.AvatarUrl;
            getBusiness.Name = model.Name;
            getBusiness.PhoneNumber = model.PhoneNumber;
            getBusiness.Email = model.Email;
            getBusiness.CreatedById = WebHelper.UserId;


            _businessRepository.Update(getBusiness);
            await _businessRepository.SaveChangesAsync();

            var businessResponse = _mapper.Map<BusinessDto>(getBusiness);
            return new SuccessResponse<BusinessDto>
            {
                Message = ResponseMessages.UpdateResponse,
                Data = businessResponse
            };
        }

        public async Task<SuccessResponse<bool>> DeleteBusinessById(Guid id)
        {
            if (id != Guid.Empty)
            {
                var getBusinessId = await _businessRepository.GetByIdAsync(id);
                if (getBusinessId != null)
                    _businessRepository.Remove(getBusinessId);
            }
            await _businessRepository.SaveChangesAsync();

            return new SuccessResponse<bool>
            {
                Message = ResponseMessages.Successful
            };
        }

        //for backoffice usage..
        public async Task<IEnumerable<Business>> GetAllBusinesses()
        {
            var get = await _businessRepository.GetAllAsync();
            return get;
        }

        async Task<User> GetUser(Guid userId)
        {
            if (userId == Guid.Empty) 
                throw new RestException(HttpStatusCode.BadRequest, ResponseMessages.ParameterCannotBeNull);

            return await _userRepository.GetByIdAsync(userId);
        }

        async void UpdateUser(User user)
        {
           _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
        }

        private async Task AddAdminUserRole(User user)
        {
            List<string> roles = new List<string>
            {
                ERole.ADMIN.ToString(),
                ERole.USER.ToString()
            };

            await _userManager.AddToRolesAsync(user, roles);
        }
    }
}