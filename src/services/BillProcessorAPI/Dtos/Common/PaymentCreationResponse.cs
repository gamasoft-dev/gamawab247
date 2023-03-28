namespace BillProcessorAPI.Dtos.Common
{
    public class PaymentCreationResponse
    {
        public string PayLink { get; set; }
        public decimal SystemCharge { get; set; }
        public string Status { get; set; }
    }
}
