using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class MessageLog: AuditableEntity
{
    public Guid Id { get; set; }
    /// <summary>
    /// This is the json string message request or response
    /// </summary>
    public string RequestResponseData { get; set; }

    public EMessageType MessageType { get; set; }
    public EMessageDirection MessageDirection { get; set; }
    public string MessageBody { get; set; }
    public string From { get; set; }
    public string To { get; set; }
    public bool IsRead { get; set; }

    // Navigation properties
    public Guid WhatsappUserId { get; set; }
    public Guid BusinessId { get; set; }

    public Business Business { get; set; }

    public WhatsappUser WhatsappUser { get; set; }
}