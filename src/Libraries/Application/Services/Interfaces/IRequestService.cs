using Application.DTOs;
using Application.Helpers;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.Interface
{
    public interface IRequestService : IAutoDependencyService
    {
        Task<RequestDto> CreateRequestConversations(RequestDto requestDto);
        Task<IEnumerable<GetRequestDto>> GetAllByBusinessId(Guid? businessId);
        Task<IEnumerable<GetRequestDto>> GetAllByIndustryId(Guid? industryId);
        Task<GetRequestDto> GetAllRequestOptionsByRequestId(Guid requestId);
        Task<GetRequestDto> GetByRequestId(Guid requestId);
    }
}