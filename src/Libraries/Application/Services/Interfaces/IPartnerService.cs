using System;
using System.Threading.Tasks;
using Application.AutofacDI;
using Application.DTOs.PartnerContentDtos;
using Application.Helpers;
using Domain.Entities;

namespace Application.Services.Interfaces
{
	public interface IPartnerService: IAutoDependencyService
	{
		Task<SuccessResponse<PartnerDto>> Create(CreatePartnerDto model);

        Task<SuccessResponse<PartnerDto>> Update(UpdatePartnerDto model);

        Task<SuccessResponse<PartnerDto>> GetById(Guid id);

        Task<SuccessResponse<PartnerDto>> GetAll(int skip, int take);

        Task<SuccessResponse<PartnerDto>> Delete(Guid id);
    }
}

