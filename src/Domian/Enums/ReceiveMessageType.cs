using System.ComponentModel;

namespace Domain.Enums
{
    public enum ReceiveMessageType
    {
        [Description("interactive")]
        Interactive=1,
        [Description("text")]
        Text = 2,
        [Description("list")]
        List = 3,
        [Description("reply")]
        ReplyButton = 4
    }
}
