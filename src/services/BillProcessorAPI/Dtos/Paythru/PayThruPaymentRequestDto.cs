namespace BillProcessorAPI.Dtos.Paythru
{
    public class PayThruPaymentRequestDto
    {
        public int amount { get; set; }
        public string productId { get; set; }
        public string transactionReference { get; set; }
        public string paymentDescription { get; set; }
        public string sign { get; set; }
        public int paymentType { get; set; }
    }
}
