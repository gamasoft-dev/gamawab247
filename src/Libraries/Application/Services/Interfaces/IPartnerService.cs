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
	public interface IPartnerService: IAutoDependencyService
	{
		Task<SuccessResponse<PartnerDto>> Create(CreatePartnerDto model);

        Task<SuccessResponse<PartnerDto>> Update(Guid id, UpdatePartnerDto model);

        Task<SuccessResponse<PartnerDto>> GetById(Guid id);

        Task<PagedResponse<IEnumerable<PartnerDto>>> GetAll(ResourceParameter parameter,
            string endPointName, IUrlHelper url);

        Task<SuccessResponse<bool>> Delete(Guid id);
    }
}

