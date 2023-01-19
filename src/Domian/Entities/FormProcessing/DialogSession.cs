using System;
using System.Collections.Generic;
using Domain.Common;
using Domain.Entities.FormProcessing.ValueObjects;
using Domain.Enums;

namespace Domain.Entities
{
    /// <summary>
    /// This should be a saved to redis with a 24 hour sliding option.
    /// Alternatively db can be used if cache system is not avaialable.
    /// </summary>
    public class DialogSession: AuditableEntity
    {
        public Guid Id { get; set; }
        public Guid BusinessId { get; set; }
        public Guid? BusinessConversationId { get; set; }
        public string WaId { get; set; }
        public string UserName { get; set; }

        /// <summary>
        /// Reference the ESessionState Enum. This specifies the state of the session
        /// ESP if it requires a form collection and processsing.
        /// </summary>
        public ESessionState SessionState { get; set; }
        public DateTime? LastInboundMessageTime { get; set; }
        public string LastMessageId { get; set; }
        public SessionFormDetail SessionFormDetails { get; set; }

    } 
}

