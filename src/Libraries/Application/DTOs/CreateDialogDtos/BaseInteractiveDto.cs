using System;

namespace Application.DTOs.CreateDialogDtos;

public class BaseInteractiveDto
{
    public Guid Id { get; set; }
    public Guid BusinessMessageId { get; set; }
    
    public string Header { get; set; }
    public string Body { get; set; }
    public string Footer { get; set; }
    public string SectionTitle { get; set; }
    
    /// <summary>
    /// Message associated with a button of a list
    /// </summary>
    public virtual ListActionDto ListAction { get; set; }
    
    // text based interactive properties
    internal virtual string KeyResponses { get; set; }
    internal virtual bool IsResponsePermitted { get; set; }
    /// <summary>
    /// Message associated with a button of a list
    /// </summary>
    public virtual ButtonActionDto ButtonAction { get; set; }
    public virtual int NextMessagePosition { get; set; }
}