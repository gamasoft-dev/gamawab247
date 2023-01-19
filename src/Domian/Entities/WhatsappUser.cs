using Domain.Common;
using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class WhatsappUser : AuditableEntity
    {
        public WhatsappUser()
        {
            MessageLogs = new List<MessageLog>();
        }
        public string WaId { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime LastMessageTime { get; set; }
        public ICollection<MessageLog> MessageLogs { get; set; }
    }
}
