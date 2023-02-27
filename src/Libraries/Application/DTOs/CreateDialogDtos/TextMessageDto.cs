using System;

namespace Application.DTOs.CreateDialogDtos
{
    public class CreateTextMessageDto
    {
        public string Header { get; set; }
        public string Body { get; set; }
        public string Footer { get; set; }
        public string KeyResponses { get; set; }
        public bool IsResponsePermitted { get; set; }

        public int NextMessagePosition { get; set; }
    }

    public class TextMessageDto: BaseInteractiveDto
    {
        // hide the members below.
        private new ListActionDto ListAction { get; set; }
       // public  string KeyResponses { get; set; }
        private new ButtonActionDto ButtonAction { get; set; }
    }

    public class UpdateTextMessageDto: CreateTextMessageDto
    {
        public Guid Id { get; set; }
    }
}
