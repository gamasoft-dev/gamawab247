using System;
using System.Threading.Tasks;
using Application.DTOs.PartnerContentDtos;
using Application.Helpers;
using Application.Services.Interfaces;

namespace Application.Services.Implementations.PartnerDetails
{
	public class PartnerContentIntegrationService: IPartnerContentIntegrationDetailsService
    {
		public PartnerContentIntegrationService()
		{
		}

        public Task<SuccessResponse<PartnerContentIntegrationDto>>
            Create(CreatePartnerContentIntegrationDto model)
        {
            throw new NotImplementedException();
        }

        public Task<SuccessResponse<PartnerContentIntegrationDto>> Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<SuccessResponse<PartnerContentIntegrationDto>> GetAll(int skip, int take)
        {
            throw new NotImplementedException();
        }

        public Task<SuccessResponse<PartnerContentIntegrationDto>> GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<SuccessResponse<PartnerContentIntegrationDto>>
            Update(UpdatePartnerContentIntegrationDto model)
        {
            throw new NotImplementedException();
        }
    }
}

