using Application.AutofacDI;
using Application.DTOs;
using Application.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IWhatsappUserService : IAutoDependencyService
    {
        Task<SuccessResponse<WhatsappUserDto>> UpsertWhatsappUser(UpsertWhatsappUserDto model);
        Task<SuccessResponse<bool>> DeleteWhatsappUserById(string waId);
        Task<SuccessResponse<WhatsappUserDto>> GetWhatsappUserByWaId(string waId);
        Task<PagedResponse<IEnumerable<WhatsappUserDto>>> GetWhatsappUsers(ResourceParameter parameter, string name, IUrlHelper urlHelper);
    }
}
