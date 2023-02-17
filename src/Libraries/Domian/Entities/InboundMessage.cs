using Domain.Common;
using System;
using Domain.Enums;

namespace Domain.Entities
{
    public class InboundMessage : AuditableEntity
    {
        public string Wa_Id { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Body { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string WhatsAppMessageId { get; set; }
        public string WhatsUserName { get; set; }
        public string Timestamp { get; set; }
        public string Type { get; set; }
        public string MsgOptionId { get; set; }
        public bool IsFirstMessageSent { get; set; }
        public string ResponseProcessingStatus { get; set; }
        public int SendAttempt { get; set; }
        public Guid BusinessId { get; set; }
        public bool CanUseNLPMapping { get; set; }
        public string Language { get; set; } = "en";
        //public bool ShouldProcessInbound { get; set; } = true;
        /// <summary>
        /// This is the Id of an initial message, this message replies to
        /// </summary>
        public string ContextMessageId { get; set; }

        public string WhatsAppId { get; set; }
        public string ErrorMessage { get; set; }
    }
}