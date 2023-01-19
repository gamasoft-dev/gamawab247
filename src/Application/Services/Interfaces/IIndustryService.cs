using Application.DTOs;
using Application.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Application.Services.Interfaces
{
    public interface IIndustryService : IAutoDependencyService
    {
        Task<SuccessResponse<IndustryDto>> CreateIndustry(CreateIndustryDto dto);
        Task<SuccessResponse<bool>> Delete(Guid id);
        Task<PagedResponse<IEnumerable<IndustryDto>>> GetAllIndustry(ResourceParameter parameter,
            string endPointName, IUrlHelper urlHelper);
        Task<SuccessResponse<IndustryDto>> GetIndustryById(Guid id);
        Task<SuccessResponse<IndustryDto>> UpdateIndustry(Guid id, UpdateIndustryDto dto);
    }
}