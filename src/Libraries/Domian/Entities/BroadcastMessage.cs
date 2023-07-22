using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Enums;

namespace Domain.Entities
{
    public class BroadcastMessage : AuditableEntity
    {
        public Guid Id { get; set; }
        public string  To { get; set; }
        public string From { get; set; }

        public string Message { get; set; }
        public string ErrorMessage { get; set; }
        public EBroadcastMessageStatus  Status { get; set; }

        public Guid BusinessId { get; set; }
        public Business Business { get; set; }

    }
}
