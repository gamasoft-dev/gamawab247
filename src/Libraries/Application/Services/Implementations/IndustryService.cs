using Application.DTOs;
using Application.Helpers;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.BusinessDtos;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;

namespace Application.Services.Implementations
{
    public class IndustryService : IIndustryService
    {
        private readonly IRepository<Industry> _repositoryIndustry;
        private readonly IMapper _mapper;

        public IndustryService(IRepository<Industry> repositoryIndustry, IMapper mapper)
        {
            _mapper = mapper;
            _repositoryIndustry = repositoryIndustry;
        }

        public async Task<SuccessResponse<IndustryDto>> CreateIndustry(CreateIndustryDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.Name) || string.IsNullOrEmpty(dto.Description))
                throw new RestException(System.Net.HttpStatusCode.BadRequest, "Validation failed", ResponseMessages.Failed);

            var model = _mapper.Map<Industry>(dto);
            model.CreatedAt = DateTime.Now;
            model.CreatedById = WebHelper.UserId;            

            await _repositoryIndustry.AddAsync(model);
            await _repositoryIndustry.SaveChangesAsync();

            var industryResponse = _mapper.Map<IndustryDto>(model);

            return new SuccessResponse<IndustryDto>
            {
                Message = ResponseMessages.CreationSuccessResponse,
                Data = industryResponse
            };
        }
        public async Task<SuccessResponse<IndustryDto>> UpdateIndustry(Guid id, UpdateIndustryDto dto)
        {
            if (dto == null || id == Guid.Empty)
                throw new RestException(System.Net.HttpStatusCode.BadRequest, "Validation failed", ResponseMessages.Failed);

            var getIndustryById = await _repositoryIndustry.GetByIdAsync(id);
            if (getIndustryById is null)
            {
                throw new RestException(System.Net.HttpStatusCode.NotFound, ResponseMessages.Failed);
            };

            getIndustryById.Name = dto.Name;
            getIndustryById.Description = dto.Description;
            getIndustryById.UpdatedAt = DateTime.Now;
            getIndustryById.CreatedById = WebHelper.UserId;

            _repositoryIndustry.Update(getIndustryById);
            await _repositoryIndustry.SaveChangesAsync();
            var resp = _mapper.Map<IndustryDto>(getIndustryById);

            return new SuccessResponse<IndustryDto>
            {
                Message = ResponseMessages.UpdateResponse,
                Data = resp
            };
        }

        public async Task<SuccessResponse<IndustryDto>> GetIndustryById(Guid id)
        {
            var industry = await _repositoryIndustry.GetByIdAsync(id);
            if (industry is null)
            {
                throw new RestException(System.Net.HttpStatusCode.NotFound, ResponseMessages.Failed);
            }

            var industryResponse = _mapper.Map<IndustryDto>(industry);

            return new SuccessResponse<IndustryDto>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = industryResponse
            };
        }

        public async Task<PagedResponse<IEnumerable<IndustryDto>>> GetAllIndustry(ResourceParameter parameter,
            string endPointName, IUrlHelper urlHelper)
        {
            var queryable = _repositoryIndustry
                .Query(x=> (string.IsNullOrEmpty(parameter.Search) 
                            || (x.Name.ToLower().Contains(parameter.Search.ToLower()) 
                                || (string.IsNullOrEmpty(x.Description) || x.Description.Contains(parameter.Search) ))));
            
            var queryProjection = queryable.ProjectTo<IndustryDto>(_mapper.ConfigurationProvider);
            
            var industriesPagedList = await PagedList<IndustryDto>.CreateAsync(queryProjection,
                parameter.PageNumber, parameter.PageSize, parameter.Sort);
            var page = PageUtility<IndustryDto>.CreateResourcePageUrl(parameter, endPointName, industriesPagedList, urlHelper);

            var response = new PagedResponse<IEnumerable<IndustryDto>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = industriesPagedList,
                Meta = new Helpers.Meta
                {
                    Pagination = page
                }
            };
            return response;
        }

        public async Task<SuccessResponse<bool>> Delete(Guid id)
        {
            var industry = await _repositoryIndustry.GetByIdAsync(id);
            if (industry is null)
            {
                throw new RestException(System.Net.HttpStatusCode.NotFound, ResponseMessages.Failed);
            }
            _repositoryIndustry.Remove(industry);
            await _repositoryIndustry.SaveChangesAsync();

            return new SuccessResponse<bool>
            {
                Message = ResponseMessages.Successful
            };
        }
    }
}
