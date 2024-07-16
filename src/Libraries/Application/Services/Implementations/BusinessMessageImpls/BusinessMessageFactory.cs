using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Application.DTOs.CreateDialogDtos;
using Application.Services.Interfaces;
using Domain.Enums;

namespace Application.Services.Implementations.BusinessMessageImpls;

public class BusinessMessageFactory: IBusinessMessageFactory
{
    private readonly IEnumerable<IBusinessMessageMgtService<BaseCreateMessageDto, BaseInteractiveDto>> _businessMessageServices;

    public BusinessMessageFactory(
        IEnumerable<IBusinessMessageMgtService<BaseCreateMessageDto, BaseInteractiveDto>> businessMessageServices)
    {
        _businessMessageServices = businessMessageServices;
    }
    
    public IBusinessMessageMgtService<BaseCreateMessageDto, BaseInteractiveDto> GetBusinessMessageImpl(string messageType = default)
    {
        messageType = messageType?.ToLower();

        if (!string.IsNullOrWhiteSpace(messageType))
        {
             var resp = _businessMessageServices
                .FirstOrDefault(x=>x.GetMessageType.ToLower() == messageType);
            return resp;
        }

        return _businessMessageServices.FirstOrDefault(x =>
            string.Equals(x.GetMessageType, EMessageType.Text.ToString(),
                StringComparison.CurrentCultureIgnoreCase));
    }

}