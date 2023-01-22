using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities.DialogMessageEntitties.ValueObjects;

namespace Domain.Entities.DialogMessageEntitties
{
    /// <summary>
    /// This is a message configuration for a reply button type of message.
    /// </summary>
    public class ReplyButtonMessage: BaseMessageTypeDetails
    {
        [Column(TypeName = "jsonb")]
        public ButtonAction ButtonAction { get; set; }
    }
}