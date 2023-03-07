using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs.PartnerContentDtos;
using Application.Helpers;
using Application.Services.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Common;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Application.Services.Implementations.PartnerDetails
{
	public class PartnerContentIntegrationService: IPartnerContentIntegrationDetailsService
    {
        private readonly IRepository<PartnerIntegrationDetails> _integrationRepo;
        private readonly IRepository<Partner> _partnerRepo;
        private readonly IMapper _mapper;
        public PartnerContentIntegrationService(IRepository<PartnerIntegrationDetails> integrationRepo,
            IMapper mapper, IRepository<Partner> partnerRepo)
        {
            _integrationRepo = integrationRepo;
            _partnerRepo = partnerRepo;
            _mapper = mapper;
        }

        public async Task<SuccessResponse<PartnerContentIntegrationDto>>
            Create(CreatePartnerContentIntegrationDto model)
        {
            if (!await _partnerRepo.ExistsAsync(x => x.Id == model.PartnerId))
                throw new RestException(System.Net.HttpStatusCode.BadRequest, "Invalid partner Id, Partner not very");

            var partnerIntegration = _mapper.Map<PartnerIntegrationDetails>(model);
            await _integrationRepo.AddAsync(partnerIntegration);
            await _integrationRepo.SaveChangesAsync();
            PartnerContentIntegrationDto partnerDto = _mapper.Map<PartnerContentIntegrationDto>(partnerIntegration);
            return new SuccessResponse<PartnerContentIntegrationDto>
            {
                Data = partnerDto,
                Success = true,
                Message = ResponseMessages.Successful
            };
        }

        public async Task<SuccessResponse<bool>> Delete(Guid id)
        {
            var getIntegrationdetails = await _integrationRepo.GetByIdAsync(id);

            if (getIntegrationdetails is null)
                throw new RestException(System.Net.HttpStatusCode.NotFound, "Invalid id, not partner content integration details found");


            _integrationRepo.Remove(getIntegrationdetails);

            await _integrationRepo.SaveChangesAsync();

            return new SuccessResponse<bool>
            {
                Message = ResponseMessages.Successful
            };
        }

        public async Task<PagedResponse<IEnumerable<PartnerContentIntegrationDto>>> GetAll(ResourceParameter parameter,
            string endPointName, IUrlHelper url)
        {
            var queryable = _integrationRepo
               .Query(x => string.IsNullOrEmpty(parameter.Search));
            if (queryable == null)
                throw new RestException(System.Net.HttpStatusCode.BadRequest, ResponseMessages.Failed);

            var queryProjection = queryable.ProjectTo<PartnerContentIntegrationDto>(_mapper.ConfigurationProvider);

            var partners = await PagedList<PartnerContentIntegrationDto>.CreateAsync(queryProjection, parameter.PageNumber, parameter.PageSize, parameter.Sort);
            var page = PageUtility<PartnerContentIntegrationDto>.CreateResourcePageUrl(parameter, endPointName, partners, url);

            var response = new PagedResponse<IEnumerable<PartnerContentIntegrationDto>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = partners,
                Meta = new Meta
                {
                    Pagination = page
                }
            };
            return response;
        }

        public async Task<SuccessResponse<PartnerContentIntegrationDto>> GetById(Guid id)
        {
            if (id == Guid.Empty)
                throw new RestException(System.Net.HttpStatusCode.BadRequest, ResponseMessages.Failed);

            if (!await _integrationRepo.ExistsAsync(x => x.Id == id))
                throw new RestException(System.Net.HttpStatusCode.NotFound, "Invalid partner content intgeration id, partner content integration not found");

            PartnerIntegrationDetails partner = await _integrationRepo.GetByIdAsync(id)
                ?? throw new RestException(System.Net.HttpStatusCode.NotFound, ResponseMessages.Failed);

            PartnerContentIntegrationDto partnerDto = _mapper.Map<PartnerContentIntegrationDto>(partner);

            return new SuccessResponse<PartnerContentIntegrationDto>
            {
                Data = partnerDto,
                Success = true,
                Message = ResponseMessages.Successful
            };
        }

        public async Task<SuccessResponse<PartnerContentIntegrationDto>>
            Update(Guid id, UpdatePartnerContentIntegrationDto model)
        {

            var partnerIntegration = await _integrationRepo.GetByIdAsync(id)
                ?? throw new RestException(System.Net.HttpStatusCode.NotFound, ResponseMessages.Failed);

            partnerIntegration.Parameters = model.Parameters;

            partnerIntegration.PartnerId = model.PartnerId;
            partnerIntegration.Headers = model.Headers;
            partnerIntegration.FullUrl = model.FullUrl;
            partnerIntegration.MetaData = model.MetaData;
            partnerIntegration.PartnerContentProcessorKey = model.PartnerContentProcessorKey;

            _integrationRepo.Update(partnerIntegration);
            await _integrationRepo.SaveChangesAsync();

            PartnerContentIntegrationDto partnerResponse = _mapper.Map<PartnerContentIntegrationDto>(partnerIntegration);
            return new SuccessResponse<PartnerContentIntegrationDto>
            {
                Data = partnerResponse,
                Success = true,
                Message = ResponseMessages.Successful
            };
        }
    }
}

