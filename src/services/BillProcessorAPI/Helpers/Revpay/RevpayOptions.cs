namespace BillProcessorAPI.Helpers.Revpay
{
    public class RevpayOptions
    {
        public string BaseUrl { get; set; }
        public string PaymentVerification { get; set; }
        public string ReferenceVerification { get; set; }
        public string ApiKey { get; set; }
        public string PolarisBankUrl { get; set; }
        public string Key { get; set; }
        public string State { get; set; }
        public string WebGuid { get; set; }
        public string ClientId { get; set; }
        public string Currency { get; set; }
        public string Type { get; set; }

        //options for charge
        public double Percentage { get; set; }
        public decimal MaximumCharge { get; set; }
        public decimal MinCharge { get; set; }


    }


}
