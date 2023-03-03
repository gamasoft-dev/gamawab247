using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.AutofacDI;
using Application.DTOs.PartnerContentDtos;
using Application.Helpers;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Application.Services.Interfaces
{
	public interface IPartnerContentIntegrationDetailsService: IAutoDependencyService
    {
        Task<SuccessResponse<PartnerContentIntegrationDto>> Create(CreatePartnerContentIntegrationDto model);

        Task<SuccessResponse<PartnerContentIntegrationDto>> Update(Guid id,UpdatePartnerContentIntegrationDto model);

        Task<SuccessResponse<PartnerContentIntegrationDto>> GetById(Guid id);

        Task<PagedResponse<IEnumerable<PartnerContentIntegrationDto>>> GetAll(ResourceParameter parameter,
            string endPointName, IUrlHelper url);

        Task<SuccessResponse<bool>> Delete(Guid id);
    }
}

