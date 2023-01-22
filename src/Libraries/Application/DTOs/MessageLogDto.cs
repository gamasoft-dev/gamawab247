using Domain.Enums;
using System;

namespace Application.DTOs
{
    public class CreateMessageLogDto
    {
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
    }

    public class MessageLogDto : CreateMessageLogDto{}

    public class UpdateMessageLogDto: CreateMessageLogDto{}
}
