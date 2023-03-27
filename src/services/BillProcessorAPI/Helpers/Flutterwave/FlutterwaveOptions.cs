namespace BillProcessorAPI.Helpers.Flutterwave
{
    public class FlutterwaveOptions
    {
        public string BaseUrl { get; set; } 
        public string CreateTransaction { get; set; }
        public string VerifyByReference { get; set; }
        public string Signature { get; set; }
        public string SecretKey { get; set; }

    }
}
