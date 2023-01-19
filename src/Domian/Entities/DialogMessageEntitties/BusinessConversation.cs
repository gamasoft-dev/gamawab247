using Domain.Common;
using System;

namespace Domain.Entities.DialogMessageEntitties
{
    public class BusinessConversation : AuditableEntity
    {
        public Guid Id { get; set; }
        public Guid BusinessId { get; set; }
        public string Title { get; set; }
        
        public BusinessMessage BusinessMessage { get; set; }
    }
    public abstract class BaseMessageTypeDetails : AuditableEntity
    {
        public Guid Id { get; set; }
        public Guid BusinessMessageId { get; set; }
        public string Header { get; set; }
        public string Body { get; set; }
        public string Footer { get; set; }

        //Navigation Property
        public BusinessMessage BusinessMessage { get; set; }
    }
}
