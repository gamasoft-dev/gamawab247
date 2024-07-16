using Application.Helpers;
using System;
using System.Threading.Tasks;

namespace Application.Services.Implementations
{
    public interface IWhatsappMessageService
    {
        Task<SuccessResponse<bool>> DisableAutomatedResponse(string waId, Guid businessId);
    }
}