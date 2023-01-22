using System;

namespace Application.DTOs
{
	#region Response Dto
	public partial class OutBoundMessageDto
    {
        public Guid Id { get; set; }
        public string RecipientType { get; set; }
        public string To { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }

        /// <summary>
        /// This is the status of message delivery.
        /// </summary>
        public string Status { get; set; }
        public Guid BusinessId { get; set; }
    }

    /// <summary>
    /// This is a response dto over an http request thus 
    /// </summary>
    public partial class OutboundMessageResponseDto
    {
        public Message[] Messages { get; set; }
        public Meta Meta { get; set; }
        public Errors[] Errors { get; set; }
	}

    public partial class Message
    {
        public string Id { get; set; }
    }

    public partial class Errors
    {
        public long Code { get; set; }
        public string Details { get; set; }
        public string Title { get; set; }
    }

    public partial class Meta
    {
        public string ApiStatus { get; set; }
        public string Version { get; set; }
    }

    #endregion

    #region Request Dtos

    public partial class Text
    {
        public string body { get; set; }
    }

	#endregion
}
