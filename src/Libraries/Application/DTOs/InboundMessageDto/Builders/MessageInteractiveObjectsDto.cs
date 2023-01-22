using System.Text.Json.Serialization;

namespace Application.DTOs.InboundMessageDto
{
    public record InteractiveButtonReply
    {
        public string type { get; set; }

        [JsonPropertyName("button_reply")]
        public button_reply button_reply { get; set; }
    }
    
    public record InteractiveListReply
    {
        public string type { get; set; }
        public list_reply list_reply { get; set; }
    }

    public record button_reply
    {
        public string id { get; set; }
        public string title { get; set; }
    }
    public record list_reply
    {
        public string id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
    }
}