using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Application.DTOs.InteractiveMesageDto.InboundMessageDto;
using Domain.Enums;

namespace Application.DTOs.OutboundMessageRequests
{
    
    /// <summary>
    /// This is the shape of the actual reply button base request sent to facebook.
    /// other properties are contained in the interactive base class
    /// </summary>
    public class ReplyButtonMessageRequest: InteractiveMessageBaseDto<ReplyButtonInteractiveDto>,
        IInteractiveMessageResponse
    {
        [JsonPropertyName("interactive")]
        public override ReplyButtonInteractiveDto interactive { get; set; }
    }

    /// <summary>
    /// Use this dto to create a reply button message and save it to the db, for a particular business or industry
    /// </summary>
    public class CreateReplyButtonMessageDto
    {
        // on the facebook, this property is known to be "interactive" not "ReplyButtonMessage"
        public ReplyButtonMessageRequest ReplyButtonInteractive { get; set; }
        public Guid BusinessId { get; set; }
        public string ConversationName { get; set; }
        public EMessagePosition EMessagePosition { get; set; }
    }
    
    #region Reply Button Interactive Message Objects
    public record ReplyButtonInteractiveDto
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
        public ActionDto action { get; set; }
    }

    public class ReplyDto
    {
        [JsonPropertyName("id")]
        public string id { get; set; }
        [JsonPropertyName("title")]
        // this should be a minimum of 2 characters and maximum of 20 characters
        public string title { get; set; }
    }

    public class ButtonDto
    {
        [JsonPropertyName("type")]
        public string type { get; set; } = "button";
        public int NextBusinessMessagePosition { get; set; }
        [JsonPropertyName("reply")]
        public ReplyDto reply { get; set; }
    }

    public class ActionDto
    {
        [JsonPropertyName("buttons")]
        public List<ButtonDto> buttons { get; set; }
    }
    #endregion
}


