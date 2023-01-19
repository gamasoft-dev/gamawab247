using System;

namespace Application.DTOs.InteractiveMesageDto
{

    public class CreateAllBusinessMessageObjectsDto<T> where T : class
    {
        public CreateBusinessConversationDto CreateBusinessConversationDto { get; set; }
        public CreateBusinessMessageDto CreateBusinessMessageDto { get; set; }
        public CreateBusinessMessageOptionDto CreateBusinessMessageOptionDto { get; set; }
        
        /// <summary>
        /// replybutton/list config dto
        /// </summary>
        public T InteractiveMessageTypeConfigDto { get; set; }

    }
    public class CreateBusinessConversationDto
    {
        public Guid BusinessId { get; set; }
        public string Title { get; set; }
    }

    public class CreateBusinessMessageDto
    {
        public Guid BusinessConversationId { get; set; }
        public Guid? MessageConfigurationId { get; set; }
        public int Position { get; set; }
        public string Name { get; set; }

        // This is the name of the message type (list, reply ButtonDto, Text) etc.
        public string MessageType { get; set; }
    }

    public class CreateReplyButtonMessageConfigDto
    {
        public Guid BusinessMessageId { get; set; }
        public string MessageType { get; set; }
        public string TextToDisplay { get; set; }
        public string Header { get; set; }
        public string Body { get; set; }
        public string Footer { get; set; }
    }

    public class CreateListMessageConfigDto
    {
        public Guid BusinessMessageId { get; set; }
        public string MessageType { get; set; }
        public string TextToDisplay { get; set; }
        public string Header { get; set; }
        public string Body { get; set; }
        public string Footer { get; set; }
    }

    public class CreateBusinessMessageOptionDto
    { 
        public Guid BusinessMessageConfigId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid? NextMessageId { get; set; }
    }
}