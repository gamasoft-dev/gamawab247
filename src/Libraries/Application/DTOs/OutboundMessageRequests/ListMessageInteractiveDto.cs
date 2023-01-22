
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Application.DTOs.OutboundMessageRequests
{
    public class ListMessageInteractiveDto
    {
        [JsonPropertyName("recipientType")]
        public string recipienttype { get; set; }
        [JsonPropertyName("to")]
        public string to { get; set; }
        [JsonPropertyName("type")]
        public string type { get; set; }
        [JsonPropertyName("interactive")]
        public Interactive interactive { get; set; }
    }
    public class Interactive
    {
        [JsonPropertyName("type")]
        public string type { get; set; }
        [JsonPropertyName("header")]
        public Header header { get; set; }
        [JsonPropertyName("body")]
        public Body body { get; set; }
        [JsonPropertyName("footer")]
        public Footer footer { get; set; }
        [JsonPropertyName("action")]
        public Action action { get; set; }
    }

    public class Action
    {
        [JsonPropertyName("button")]
        public string button { get; set; }
        [JsonPropertyName("sections")]
        public IEnumerable<Sections> sections { get; set; }
    }

    public class Sections
    {
        [JsonPropertyName("title")]
        public string title { get; set; }
        [JsonPropertyName("rows")]
        public List<Rows> rows { get; set; }
    }

    public class Rows
    {
        [JsonPropertyName("id")]
        public string id { get; set; }
        [JsonPropertyName("title")]
        public string title { get; set; }
        [JsonPropertyName("description")]
        public string description { get; set; }
    }

    public class Footer
    {
        [JsonPropertyName("text")]
        public string text { get; set; }
    }
    public class Body
    {
        [JsonPropertyName("text")]
        public string text { get; set; }
    }

    public class Header
    {
        [JsonPropertyName("type")]
        public string type { get; set; }
        [JsonPropertyName("text")]
        public string text { get; set; }
    }
}
