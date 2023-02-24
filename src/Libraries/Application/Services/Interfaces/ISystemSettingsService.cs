using System.Threading.Tasks;
using Application.AutofacDI;
using Application.DTOs;
using Application.Helpers;

namespace Application.Services.Interfaces
{
    public interface ISystemSettingsService: IAutoDependencyService
    {
        Task<SuccessResponse<SystemSettingsDto>> CreateUpdateSystemSettings(CreateSystemSetttingsDto dto);
        Task<SuccessResponse<SystemSettingsDto>> GetSystemSettings();
    }
}