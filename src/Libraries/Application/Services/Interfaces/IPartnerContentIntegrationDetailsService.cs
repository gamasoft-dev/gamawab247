using System;
using System.Threading.Tasks;
using Application.AutofacDI;
using Application.DTOs.PartnerContentDtos;
using Application.Helpers;
using Domain.Entities;

namespace Application.Services.Interfaces
{
	public interface IPartnerContentIntegrationDetailsService: IAutoDependencyService
    {
        Task<SuccessResponse<PartnerContentIntegrationDto>> Create(CreatePartnerContentIntegrationDto model);

        Task<SuccessResponse<PartnerContentIntegrationDto>> Update(UpdatePartnerContentIntegrationDto model);

        Task<SuccessResponse<PartnerContentIntegrationDto>> GetById(Guid id);

        Task<SuccessResponse<PartnerContentIntegrationDto>> GetAll(int skip, int take);

        Task<SuccessResponse<PartnerContentIntegrationDto>> Delete(Guid id);
    }
}

