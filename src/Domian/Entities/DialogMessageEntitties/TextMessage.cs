namespace Domain.Entities.DialogMessageEntitties
{
    /// <summary>
    /// This is a message configuration for basic text message.d
    /// </summary>
    public class TextMessage: BaseMessageTypeDetails
    {
        public int NextMessagePosition { get; set; }
        // a comma separated set of keys anticipated as responses if the text is replied to.
        public string KeyResponses { get; set; }
        public bool IsResponsePermitted { get; set; }
    }
}