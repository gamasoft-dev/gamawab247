using System;

namespace Application.DTOs
{
    public class DialogSessionDto
    {
        public Guid BusinessId { get; set; }
        public Guid? BusinessConversationId { get; set; }
        public string WaId { get; set; }
        public string UserName { get; set; }
        public int SessionState { get; set; }
    }

    public class CreateDialogSessionDto : DialogSessionDto { }
    public class UpdateDialogSessionDto : DialogSessionDto { }
}
