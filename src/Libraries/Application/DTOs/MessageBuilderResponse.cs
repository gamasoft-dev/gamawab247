using Application.DTOs.InboundMessageDto;
using Application.Helpers;
using Domain.Entities;

namespace Application.DTOs
{
    public class MessageBuilderResponse<T>
    {
		public RestException RequestException { get; set; }
		public string Message { get; set; }
        public int Code { get; set; }
        public T MessageData { get; set; }
        public Business Business { get; set; }
		public BusinessMessageSettings BusinessMessageSettings { get; set; }
	}

    public class MessageBuilderRequest
	{
		public Business Business { get; set; }
		public BusinessMessageSettings BusinessMessageSettings { get; set; }

		public TextNotificationDto InboundText { get; set; }
	}
}
