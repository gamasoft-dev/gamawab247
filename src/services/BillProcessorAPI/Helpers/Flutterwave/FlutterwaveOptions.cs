namespace BillProcessorAPI.Helpers.Flutterwave
{
    public class FlutterwaveOptions
    {
        public string BaseUrl { get; set; } 
        public string RedirectUrl { get; set; }
        public string CreateTransaction { get; set; }
        public string VerifyByTransactionID { get; set; }
        public string VerifyByReference { get; set; }
        public decimal MinimumPayableAmount { get; set; }
        public string Signature { get; set; }
        public string SecretKey { get; set; }
        public string ExistingAppUrl { get; set; }
        public string ResendFailedWebhook { get; set; }
        public string ResendWebhookHeader { get; set; }
        public string ResendWebhookHeaderValue { get; set; }

    }
}
