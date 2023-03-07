using Application.AutofacDI;
using Application.DTOs.BusinessDtos;
using Application.Helpers;
using Domain.Entities.FormProcessing;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Application.Services.Interfaces.FormProcessing
{
    public interface IBusinessFormService : IAutoDependencyService
    {
        Task<SuccessResponse<BusinessFormDto>> CreateBusinessForm(CreateBusinessFormDto model);
        Task<SuccessResponse<bool>> UpdateBusinessForm(Guid id, UpdateBusinessFormDto model);
        Task<SuccessResponse<BusinessFormDto>> GetBusinessFormById(Guid id);
        Task<PagedResponse<IEnumerable<BusinessFormDto>>> GetAllByBusinessId(Guid businessId, string search,
            string name, ResourceParameter parameter, IUrlHelper urlHelper);
        //Task<SuccessResponse<BusinessForm>> GetBusinessFormByBusinessId(Guid businessId);
        internal Task<BusinessForm> GetBusinessFormFisrtOrDefault(Expression<Func<BusinessForm, bool>> expression);
    }
}