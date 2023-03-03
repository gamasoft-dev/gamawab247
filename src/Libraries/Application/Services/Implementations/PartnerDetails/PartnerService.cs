using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.BusinessDtos;
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
    public class PartnerService : IPartnerService
    {
        private readonly IRepository<Partner> _partnerRepo;
        private readonly IMapper _mapper;
        public PartnerService(IRepository<Partner> partnerRepo, IMapper mapper)
        {
            _partnerRepo = partnerRepo;
            _mapper = mapper;
        }
        public async Task<SuccessResponse<PartnerDto>> Create(CreatePartnerDto model)
        {
            if (model == null || string.IsNullOrEmpty(model.Name))
                throw new RestException(System.Net.HttpStatusCode.BadRequest, ResponseMessages.PayLoadCannotBeNull);

            Partner partner = _mapper.Map<Partner>(model);
            await _partnerRepo.AddAsync(partner);
            await _partnerRepo.SaveChangesAsync();
            PartnerDto partnerDto = _mapper.Map<PartnerDto>(partner);
            return new SuccessResponse<PartnerDto>
            {
                Data = partnerDto,
                Success = true,
                Message = ResponseMessages.Successful
            };
        }

        public async Task<SuccessResponse<bool>> Delete(Guid id)
        {
            if (id != Guid.Empty)
            {
                var getPartner = await _partnerRepo.GetByIdAsync(id);

                if (getPartner != null)
                    _partnerRepo.Remove(getPartner);
            }
            await _partnerRepo.SaveChangesAsync();

            return new SuccessResponse<bool>
            {
                Message = ResponseMessages.Successful
            };
        }

        public async Task<PagedResponse<IEnumerable<PartnerDto>>> GetAll(ResourceParameter parameter,
            string endPointName, IUrlHelper url)
        {
            var queryable = _partnerRepo
               .Query(x => string.IsNullOrEmpty(parameter.Search)
                           || x.Name.ToLower().Contains(parameter.Search.ToLower()));
            if (queryable == null)
                throw new RestException(System.Net.HttpStatusCode.BadRequest, ResponseMessages.Failed);

            var queryProjection = queryable.ProjectTo<PartnerDto>(_mapper.ConfigurationProvider);

            var partners = await PagedList<PartnerDto>.CreateAsync(queryProjection, parameter.PageNumber, parameter.PageSize, parameter.Sort);
            var page = PageUtility<PartnerDto>.CreateResourcePageUrl(parameter, endPointName, partners, url);

            var response = new PagedResponse<IEnumerable<PartnerDto>>
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

        public async Task<SuccessResponse<PartnerDto>> GetById(Guid id)
        {
            if (id == Guid.Empty)
                throw new RestException(System.Net.HttpStatusCode.BadRequest, ResponseMessages.Failed);
            Partner partner = await _partnerRepo.GetByIdAsync(id)
                ?? throw new RestException(System.Net.HttpStatusCode.NotFound, ResponseMessages.Failed);
            PartnerDto partnerDto = _mapper.Map<PartnerDto>(partner);
            return new SuccessResponse<PartnerDto>
            {
                Data = partnerDto,
                Success = true,
                Message = ResponseMessages.Successful
            };
        }

        public async Task<SuccessResponse<PartnerDto>> Update(Guid id,UpdatePartnerDto model)
        {
            if (id== Guid.Empty || model == null)
                throw new RestException(System.Net.HttpStatusCode.BadRequest, ResponseMessages.Failed);

            var partner = await _partnerRepo.GetByIdAsync(id)
                ?? throw new RestException(System.Net.HttpStatusCode.NotFound, ResponseMessages.Failed);

            partner.Name = model.Name;
            partner.Description = model.Description;
            partner.BusinessId = model.BusinessId;

            _partnerRepo.Update(partner);
            await _partnerRepo.SaveChangesAsync();

            PartnerDto partnerResponse = _mapper.Map<PartnerDto>(partner);
            return new SuccessResponse<PartnerDto>
            {
                Data = partnerResponse,
                Success = true,
                Message = ResponseMessages.Successful
            };
        }
    }
}
