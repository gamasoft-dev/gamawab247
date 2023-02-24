using System;
using System.Threading.Tasks;
using Application.DTOs;
using Application.DTOs.BusinessDtos;
using Application.Helpers;
using Domain.Entities;

namespace Application.Services.Interfaces
{
    public interface IBusinessSettingsService : IAutoDependencyService
    {
        Task<SuccessResponse<BusinessMessageSettings>> ProcessCreateBusinessSetting(Guid id, CreateBusinessSetupDto dto);
        Task<SuccessResponse<BusinessMessageSettings>> GetById(Guid id);
        Task<PagedList<BusinessMessageSettings>> GetAllBusinessSetups(string search, int? skip = 0,
            int? take = int.MaxValue);
        Task DeleteBusinessById(Guid id);
        Task<SuccessResponse<BusinessMessageSettings>> GetByBusinessId(Guid businessGuid);
        Task<SuccessResponse<bool>> Update(BusinessMessageSettings model);
        Task<bool> ProcessTestCounter(Guid id);
        Task<string> GenerateBusinessWebHook(Guid businessId);
    }
}