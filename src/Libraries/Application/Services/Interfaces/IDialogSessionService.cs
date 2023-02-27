using Application.AutofacDI;
using Application.DTOs;
using Application.Helpers;
using System;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IDialogSessionService : IAutoDependencyService
    {
        Task<SuccessResponse<DialogSessionDto>> CreateUserFormData(CreateDialogSessionDto model);
        Task<SuccessResponse<DialogSessionDto>> UpdateUserFormData(UpdateDialogSessionDto model);
        Task<SuccessResponse<DialogSessionDto>> GetUserFormDataById(Guid id);
    }
}
