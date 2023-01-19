using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Application.DTOs.InboundMessageDto
{
    /// <summary>
    /// Inbound message request base interface structure.
    /// Note: T is the specific inbound message type's object
    /// </summary>
    public interface IInboundMessageRequestDto<T>
    {
        [JsonPropertyName("contacts")]
        public List<Contact> Contacts { get; set; }
        
        [JsonPropertyName("messages")]
        public List<T> Messages { get; set; }
    }

    /// <summary>
    /// This is the general abstract object of inbound message request structure.
    /// All specific inbound message objects by types can inherit this and override the members.
    /// </summary>
    public abstract class InboundMessageRequestDto<T>:
        IInboundMessageRequestDto<T> where T : IInboundMessageObjectDto
    {
        [JsonPropertyName("contacts")]
        public virtual List<Contact> Contacts { get; set; }
        
        [JsonPropertyName("messages")]
        public virtual List<T> Messages { get; set; }
    }
    
    
    /// <summary>
    /// This is the general interface for all inbound message objects
    /// Specific inbounds message types may have more properties but these below are general.
    /// All message object of respective inbound message types should implement this.
    /// </summary>
    public interface IInboundMessageObjectDto 
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("from")]
        public string From { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }
    }
    
    /// <summary>
    /// This is the general abstract inbound message object structure.
    /// Specific inbounds message types may have more properties but these below are general.
    /// </summary>
    public abstract class InboundMessageObjectDto: IInboundMessageObjectDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        [JsonPropertyName("recipient_type")]
        public string RecipientType { get; set; }
        
        [JsonPropertyName("from")]
        public string From { get; set; }

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
    
    
}