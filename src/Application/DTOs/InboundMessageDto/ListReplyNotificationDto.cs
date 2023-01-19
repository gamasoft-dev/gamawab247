using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Application.DTOs.InboundMessageDto
{
    /// <summary>
    /// This is the shape of the request 360 dialog sends when a user responds to a
    /// list based message initially sent.
    /// </summary>
    public class ListReplyNotificationDto : InboundMessageRequestDto<ListReplyMessageObject>
    {
        [JsonPropertyName("messages")]
        public override List<ListReplyMessageObject> Messages { get; set; }
    }
    
    public class ListReplyMessageObject : InboundMessageObjectDto
    {
        public InteractiveContextDto context { get; set; }

        public string group_id { get; set; }

        [JsonPropertyName("interactive")]
        public InteractiveListReply Interactive { get; set; }
    }
}