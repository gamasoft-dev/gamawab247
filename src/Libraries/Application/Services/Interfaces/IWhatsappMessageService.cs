using Application.AutofacDI;
using Application.Helpers;
using System;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IWhatsappMessageService: IAutoDependencyService
    {
        Task<SuccessResponse<bool>> DisableAutomatedResponse(string waId, Guid businessId);
        Task<SuccessResponse<bool>> EnableAutomatedResponse(string waId, Guid businessId);
    }
}