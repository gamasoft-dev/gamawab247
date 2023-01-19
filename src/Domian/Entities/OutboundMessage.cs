using System;
using Domain.Common;
using Domain.Entities.DialogMessageEntitties;
using Domain.Enums;

namespace Domain.Entities
{
    public class OutboundMessage: AuditableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } 
        public bool IsFirstMessageSent { get; set; }
        public string MessageType { get; set; }
        public string RecipientWhatsappId { get; set; }
        public string Body { get; set; }
        public string From { get; set; }
        public string WhatsAppMessageId { get; set; }
        public string Type { get; set; }
        public Guid BusinessId { get; set; }
        public Guid? BusinessMessageId { get; set; }
        
        //Navigation properties
        public BusinessMessage BusinessMessage { get; set; }
    }
}