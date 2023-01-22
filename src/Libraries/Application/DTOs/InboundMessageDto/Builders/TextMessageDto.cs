using System;

namespace Application.DTOs.InteractiveMesageDto.CreateMessageRequestDto
{
    public class TextMessageDto
    {
        public Guid BusinessId { get; set; }
        public string RecipientType { get; set; }
        public string To { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }
    }
}