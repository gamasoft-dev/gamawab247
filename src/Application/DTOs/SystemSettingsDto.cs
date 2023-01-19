using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public  class SystemSettingsDto
    {
        public Guid Id { get; set; }
        public string BaseWebhook { get; set; }
        public int MaxTestCount { get; set; }
    }

    public class CreateSystemSetttingsDto
    {
        public string BaseWebHook { get; set; }

        public int MaxTestCount { get; set; }
    }
}
