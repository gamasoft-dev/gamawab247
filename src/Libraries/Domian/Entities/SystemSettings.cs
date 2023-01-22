using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class SystemSettings : AuditableEntity
    {
        public Guid Id { get; set; }
        public string BaseWebhook { get; set; }
        public int MaxTestCount { get; set; } //set global counts for business on on-boarding
    }
}
