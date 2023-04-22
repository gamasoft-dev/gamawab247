using Application.AutofacDI;
using Application.DTOs;
using Application.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IMessageLogService : IAutoDependencyService
    {
        Task<SuccessResponse<MessageLogDto>> CreateMessageLog(CreateMessageLogDto model);
        Task<SuccessResponse<MessageLogDto>> UpdateMessageLog(UpdateMessageLogDto model);
        Task<SuccessResponse<bool>> DeleteMessageLogById(Guid id);
        Task<SuccessResponse<MessageLogDto>> GetMessageLogById(Guid id);
        Task<PagedResponse<IEnumerable<MessageLogDto>>> GetMessageLogs(ResourceParameter parameter, string name, IUrlHelper urlHelper);
        Task<PagedResponse<IEnumerable<MessageLogDto>>> GetMessageLogsByWaId(Guid waId, ResourceParameter parameter, string name, IUrlHelper urlHelper);
        Task<PagedResponse<IEnumerable<MessageLogDto>>> GetMessageLogsByPhoneNumber(string waId, ResourceParameter parameter, string name, IUrlHelper urlHelper);

    }
}
