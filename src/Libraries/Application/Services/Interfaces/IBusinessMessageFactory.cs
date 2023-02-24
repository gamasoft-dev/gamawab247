using Application.DTOs.CreateDialogDtos;
using Application.AutofacDI;

namespace Application.Services.Interfaces;

public interface IBusinessMessageFactory: IAutoDependencyService
{
    IBusinessMessageMgtService<BaseCreateMessageDto, BaseInteractiveDto> GetBusinessMessageImpl(string messageType);
}