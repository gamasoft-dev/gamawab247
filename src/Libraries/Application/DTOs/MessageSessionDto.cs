using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class CreateMessageSessionDto
    {
        public Guid BusinessId { get; set; }
        public string MessageType { get; set; }
        public int LastMessagePosition { get; set; }
        public string RecipientWhatsappId { get; set; }
        public DateTime? StartDateTime { get; set; }
    }

    public class CreateMessageSessionLogDto
    {
        public bool IsFirstMessageSent { get; set; }
        public Guid MessageSessionId { get; set; }
        public EMessageDirection Direction { get; set; }
        public Guid? BusinessMessageId { get; set; }
        public Guid? MsgOptionId { get; set; }
        public string MessageType { get; set; }
        public int LastMessagePosition { get; set; }
    }
}
