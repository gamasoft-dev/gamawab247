using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Application.DTOs.InboundMessageDto;

public class DefaultMessageNotificationDto 
{
    [JsonPropertyName("contacts")]
    public List<Contact> Contacts { get; set; }
        
    [JsonPropertyName("messages")]
    public List<dynamic> Messages { get; set; }
}