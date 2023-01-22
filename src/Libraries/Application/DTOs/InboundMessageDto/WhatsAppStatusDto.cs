using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.InboundMessageDto
{
    public class WhatsAppStatusDto
    {
        public List<Status> statuses { get; set; }
    }

    public class Status
    {
        public dynamic conversation { get; set; }
        public string id { get; set; }
        public dynamic pricing { get; set; }
        public string recipient_id { get; set; }
        public string status { get; set; }
        public string timestamp { get; set; }
        public string type { get; set; }
    }
}
