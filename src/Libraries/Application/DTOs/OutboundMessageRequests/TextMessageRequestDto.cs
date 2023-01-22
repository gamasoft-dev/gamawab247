using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.CreateDialogDtos;

namespace Application.DTOs.OutboundMessageRequests
{
    public class TextMessageRequestDto
    {
        public bool previewUrl { get; set; }
        public string recipient_type { get; set; }
        public string to { get; set; }
        public string type { get; set; }
        public Text text { get; set; }
    }
}
