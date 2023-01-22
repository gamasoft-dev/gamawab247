using System;
using System.Collections.Generic;

namespace Application.DTOs.CreateDialogDtos;

public class ListMessageDto : BaseInteractiveDto
{
    // intentionally hide the inherited member of the base class that is not needed.
    private ButtonActionDto ButtonAction { get; set; }
    private int NextMessagePosition { get; set; }
}

public class CreateListMessageDto: BaseCreateMessageDto
{
    // this would hid the ButtonAction member of the base class.
    private  ButtonActionDto ButtonAction { get; set; }
    private Guid BusinessMessageId { get; set; }
    private Guid BusinessId { get; set; }
}

#region Value Objects
public class ListActionDto
{
    public string Button { get; set; }
    public List<SectionDto> Sections { get; set; }
}

public class SectionDto
{
    public string Title { get; set; }
    public List<RowDto> Rows { get; set; }
}

public class RowDto
{
    /// <summary>
    /// This should be stringified guid id of the created row.
    /// </summary>
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int NextBusinessMessagePosition { get; set; }
}


public class UpdateListMessageDto: CreateListMessageDto
{
    public Guid Id { get; set; }
}


#endregion