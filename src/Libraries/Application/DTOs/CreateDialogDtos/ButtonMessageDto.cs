using System;
using System.Collections.Generic;
using Application.DTOs.OutboundMessageRequests;

namespace Application.DTOs.CreateDialogDtos;

public class CreateButtonMessageDto: BaseCreateMessageDto
{
    internal ListActionDto ListAction { get; set; }
}

public class ButtonMessageDto: BaseInteractiveDto
{
    public string ButtonMessage { get; set; }
    
    // hide the unconcerned inherited member of the base class.
    private ListActionDto ListAction { get; set; }
}

public class ButtonActionDto
{
    public List<ButtonDto> Buttons { get; set; }
}