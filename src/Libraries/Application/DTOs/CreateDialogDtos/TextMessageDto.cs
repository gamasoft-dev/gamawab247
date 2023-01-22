using System;

namespace Application.DTOs.CreateDialogDtos
{
    public class CreateTextMessageDto
    {
        public string Header { get; set; }
        public string Body { get; set; }
        public string Footer { get; set; }

        public int NextMessagePosition { get; set; }
    }

    public class TextMessageDto: BaseInteractiveDto
    {
        // hide the members below.
        private string ButtonMessage { get; set; }
        private ListActionDto ListAction { get; set; }
        public ButtonActionDto ButtonAction { get; set; }
    }

    public class UpdateTextMessageDto: CreateTextMessageDto
    {
        public Guid Id { get; set; }
    }
}
