﻿using System;
using Domain.Common;
using Domain.Entities.FormProcessing;

namespace Domain.Entities.DialogMessageEntitties
{
    /// <summary>
    /// A business message for a particular message type.
    /// </summary>
    public class BusinessMessage : AuditableEntity
    {
        public Guid Id { get; set; }
        public Guid? BusinessConversationId { get; set; }
        public int Position { get; set; } = 2;
        public string Name { get; set; }

        /// <summary>
        /// Use this to denote if a follow up message needs to be sent to the user immediately this is sent.
        /// </summary>
        public bool HasFollowUpMessage { get; set; }

        /// <summary>
        /// This is the parent of the message this message follow. This is optional.
        /// </summary>
        public Guid? FollowParentMessageId { get; set; }
        
        // This is the name of the message type (list, Reply Button, Text) etc.
        public string MessageType { get; set; }
        public string RecipientType { get; set; }
        
        // This represents the Id of either a list of button message related to this business message
        public Guid? InteractiveMessageId { get; set; }
        
        // navigation properties
        public BusinessConversation BusinessConversation { get; set; }
        public Business Business { get; set; }
        public Guid BusinessId { get; set; }

        // form processing relating properties
        public bool ShouldTriggerFormProcessing { get; set; }
        public Guid? BusinessFormId { get; set; }

        public BusinessForm BusinessForm { get; set; }
    }
}