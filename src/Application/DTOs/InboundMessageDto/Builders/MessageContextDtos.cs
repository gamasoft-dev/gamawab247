using System.Text.Json.Serialization;

namespace Application.DTOs.InboundMessageDto
{
   #region Context Base interfaces and abstract objects
    /// <summary>
    /// This is the generic interface of all context objects for inbound messages.
    /// Specific inbound messages types like direct text message, message replies, button interactive replies, list interactive replies etc
    /// would implement this and provide further specific context properties
    /// </summary>
    public interface IInboundContextDto
    {
        [JsonPropertyName("from")]
        public string From { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }
    }

    /// <summary>
    /// Abstract base implementation for context value object.
    /// </summary>
    public abstract class InboundContextDto: IInboundContextDto
    {
        [JsonPropertyName("from")]
        public string From { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
    
    /// <summary>
    /// This is the interface for interactive message contexts.
    /// This satisfies only button and list replies inbound message
    /// </summary>
    public interface IInteractiveContextDto: IInboundContextDto
    {
        public string group_id { get; set; }
        public string[] mentions { get; set; }
    }
    
    #endregion
    
    public class InteractiveContextDto : IInteractiveContextDto
    {
        [JsonPropertyName("from")]
        public string From { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        public string group_id { get; set; }
        public string[] mentions { get; set; }
    }
    
    public class TextMessageContextDto: InboundContextDto
    {
        public ReferredProduct referred_product { get; set; }
    }

    #region Context Child objects
    public record ReferredProduct
    {
        public string catalog_id { get; set; }
        public string product_retailer_id { get; set; }
    }
    
    #endregion
}