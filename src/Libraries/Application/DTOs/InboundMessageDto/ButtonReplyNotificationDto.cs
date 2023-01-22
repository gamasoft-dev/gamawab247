using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Application.DTOs.InboundMessageDto
{
    /// <summary>
    /// This is the shape of the button reply notification request that 360 Dialog sends
    /// When a custom replies to a sent button message
    /// </summary>
    public class ButtonReplyNotificationDto : InboundMessageRequestDto<ButtonReplyMessageObjectDto>
    {
        [JsonPropertyName("messages")]
        public override List<ButtonReplyMessageObjectDto> Messages { get; set; }
    }
    
    public class ButtonReplyMessageObjectDto : InboundMessageObjectDto
    {
        public InteractiveContextDto context { get; set; }

        public string group_id { get; set; }

        [JsonPropertyName("interactive")]
        public InteractiveButtonReply interactive { get; set; }
    }
}