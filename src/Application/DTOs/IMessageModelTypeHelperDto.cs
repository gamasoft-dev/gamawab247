using Application.DTOs.InboundMessageDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class IMessageModelTypeHelperDto
    {
        public TextNotificationDto TextNotification { get; set; }
        public ListReplyNotificationDto ListReplyNotificationDto { get; set; }
        public ButtonReplyNotificationDto ButtonReplyNotificationDto { get; set; }
    }
}
