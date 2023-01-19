using System.Text.Json.Serialization;

namespace Application.DTOs.InboundMessageDto
{
    public class Contact
    {
        [JsonPropertyName("profile")] public WhatsappProfile profile { get; set; }

        [JsonPropertyName("wa_id")] public string wa_id { get; set; } // save this
    }
    
    public class WhatsappProfile
    {
        [JsonPropertyName("name")] public string name { get; set; } //save this
    }
}