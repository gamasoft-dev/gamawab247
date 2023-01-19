using System;
using System.Collections.Generic;
using Application.DTOs.OutboundMessageRequests;

namespace Application.DTOs.InteractiveMesageDto
{
    public class GetReplyButtonMessageDto
    {
        public Guid BusinessId { get; set; }
        public Guid BusinessMessageId { get; set; }
        public string Type { get; set; }
        public int Position { get; set; }
        public string Header { get; set; }
        public string HeaderType { get; set; }
        public string Body { get; set; }
        public string Footer { get; set; }
        public List<ReplyDto> Buttons { get; set; }
    }
}