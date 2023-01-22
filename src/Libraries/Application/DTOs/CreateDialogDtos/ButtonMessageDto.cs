using System;
using System.Collections.Generic;
using Application.DTOs.OutboundMessageRequests;

namespace Application.DTOs.CreateDialogDtos;

public class CreateButtonMessageDto: BaseCreateMessageDto
{
    private ListActionDto ListAction { get; set; }
    private int NextMessagePosition { get; set; }
    private Guid BusinessMessageId { get; set; }
    private Guid BusinessId { get; set; }
}

public class ButtonMessageDto: BaseInteractiveDto
{
    public override string ButtonMessage { get; set; }
    
    // hide the unconcerned inherited member of the base class.
    private ListActionDto ListAction { get; set; }
    private int NextMessagePosition { get; set; }
}

public class ButtonActionDto
{
    public List<ButtonDto> Buttons { get; set; }
}