using System;
using System.Collections.Generic;
using Domain.Common;

namespace Domain.Entities
{
    public class MessagePositionPhraseMap: AuditableEntity
    {
        public MessagePositionPhraseMap()
        {
            PhrasesAndPostion = new HashSet<string>();
        }
        public Guid BusinessId { get; set; }
        public Guid? BusinessConversationId { get; set; }

        /// <summary>
        /// Note that words and phrases saved into this column must follow structure or key and value map.
        /// Using : as the separator between the key and value e.g hello:2
        /// Note: key is the word or phrase while value is the position.
        /// </summary>
        public ICollection<string> PhrasesAndPostion { get; set; }
    }
}

