namespace BillProcessorAPI.Entities
{
    public class Receipt
    {
        public Guid Id { get; set; }
        public Guid TransactionId { get; set; }
        public string TransactionDate { get; set; }
        public string GateWay { get; set; }
        public string PaymentRef { get; set; }
        public string GatewayTransactionReference { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal AmountDue { get; set; }
        public string ReceiptUrl { get; set; }
        public BillTransaction Transaction { get; set; }
    }
}
