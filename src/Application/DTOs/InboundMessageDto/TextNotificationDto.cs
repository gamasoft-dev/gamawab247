using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Application.DTOs.CreateDialogDtos;

namespace Application.DTOs.InboundMessageDto
{
    /// <summary>
    /// This is the shape of the message received from 360 Dialog via webhook
    /// specifically for direct text based message.
    /// </summary>
    public class TextNotificationDto: InboundMessageRequestDto<TextMessageObjectDto>
    {
        [JsonPropertyName("messages")]
        public override List<TextMessageObjectDto> Messages { get; set; }
        
    }

    public class TextMessageObjectDto: InboundMessageObjectDto
    {
        [JsonPropertyName("text")]
        public Text text { get; set; }

        [JsonPropertyName("context")]
        public TextMessageContextDto context { get; set; }
    }
}