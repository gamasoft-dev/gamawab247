namespace BillProcessorAPI.Helpers.Paythru
{
    public class PaythruOptions
    {
        public string ApiKey { get; set; }
        public string Secret { get; set; }
        public string CreateTransactionUrl { get; set; }
        public decimal TransactionCharge { get; set; }
        public string ProductId { get; set; }
        public int PaymentType { get; set; }
        public decimal MinimumPayableAmount { get; set; }
        public string LoginUrl { get; set; }
    }
}
