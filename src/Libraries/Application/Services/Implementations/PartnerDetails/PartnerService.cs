using System;
using System.Threading.Tasks;
using Application.DTOs.PartnerContentDtos;
using Application.Helpers;
using Application.Services.Interfaces;

namespace Application.Services.Implementations.PartnerDetails
{
	public class PartnerService: IPartnerService
	{
		public PartnerService()
		{
		}

        public Task<SuccessResponse<PartnerDto>> Create(CreatePartnerDto model)
        {
            throw new NotImplementedException();
        }

        public Task<SuccessResponse<PartnerDto>> Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<SuccessResponse<PartnerDto>> GetAll(int skip, int take)
        {
            throw new NotImplementedException();
        }

        public Task<SuccessResponse<PartnerDto>> GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<SuccessResponse<PartnerDto>> Update(UpdatePartnerDto model)
        {
            throw new NotImplementedException();
        }
    }
}

