using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities.DialogMessageEntitties.ValueObjects;

namespace Domain.Entities.DialogMessageEntitties
{
    /// <summary>
    /// 
    ///  This is a message configuration for list type of message
    /// </summary>
    public class ListMessage: BaseMessageTypeDetails
    {
        public string SectionTitle { get; set; }
        public string ButtonMessage { get; set; }
       
        [Column(TypeName = "jsonb")]
        public ListAction ListAction { get; set; }
    }
}