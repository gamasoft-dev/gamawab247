using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.AutofacDI;
using Application.DTOs;
using Application.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Application.Services.Interfaces
{
    public interface IBroadcastMessageService : IAutoDependencyService
    {
        Task<SuccessResponse<BroadcastMessageDto>> CreateBroadcastMessage(CreateBroadcastMessageDto model);
        Task<SuccessResponse<BroadcastMessageDto>> UpdateBroadcastMessage(Guid id, UpdateBroadcastMessageDto model);
        Task<SuccessResponse<bool>> DeleteBroadcastMessage(Guid id);
        Task<SuccessResponse<BroadcastMessageDto>> GetBroadcastMessageById(Guid id);
        Task<PagedResponse<IEnumerable<BroadcastMessageDto>>> GetAllBroadcastMessage(ResourceParameter parameter, string name, IUrlHelper urlHelper);
        Task<PagedResponse<IEnumerable<BroadcastMessageDto>>> GetBroadcastMessageByBusinessId(Guid businessId, ResourceParameter parameter, string name, IUrlHelper urlHelper);
        Task<PagedResponse<IEnumerable<BroadcastMessageDto>>> GetAllPendingBroadcastMessage(ResourceParameter parameter, string name, IUrlHelper urlHelper);
    }
}
