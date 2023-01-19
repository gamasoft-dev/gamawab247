namespace Application.DTOs
{
    public class    Dialog360Settings
    {
        public string BaseUrl { get; set; }
		public string AuthorizationName { get; set; }
    }
    public class SystemSettingsConfig
    {
        public string AppBaseUrl { get; set; }
        public string CLIENT_URL { get; set; }
        public string AZURE_STORAGE_CONNECTION { get; set; }
        public string AZURE_CONTAINER_NAME { get; set; }
        public int ConversationValidityPeriod { get; set; }
        public int MaxSendAttempt { get; set; } = 3;
        public int BgDelayIntervalMilliseconds { get; set; }
        public int MessageResponseInMilliseconds { get; set; }
        public int WebhookConfigTo360InMilliseconds { get; set; }
    }
}