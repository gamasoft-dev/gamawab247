using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.AutofacDI;
using Application.DTOs.BusinessDtos;
using Application.Helpers;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Application.Services.Interfaces
{
    public interface IBusinessService : IAutoDependencyService
    {
        Task<SuccessResponse<BusinessDto>> ProcessCreateBusiness(CreateBusinessDto businessDto);
        //Task<Business> GetBusinessByOrganizationId(Guid orgId);
        Task<SuccessResponse<BusinessDto>> GetBusinessByBusinessId(Guid businessId);
        Task<SuccessResponse<BusinessDto>> UpdateBusinessInfo(Business model);
        Task<SuccessResponse<bool>> DeleteBusinessById(Guid id);
        Task<PagedResponse<IEnumerable<BusinessDto>>> GetAllBusinesses(ResourceParameter parameter, string endPointName,
            IUrlHelper url);
        Task<SuccessResponse<IEnumerable<BusinessDto>>> GetAllBusinessesByUserEmail(string email);
    }
}