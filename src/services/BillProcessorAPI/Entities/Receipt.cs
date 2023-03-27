using BillProcessorAPI.Enums;

namespace BillProcessorAPI.Entities
{
    public class Receipt
    {
        public Guid Id { get; set; }
        public DateTime TransactionDate { get; set; }
        public EGatewayType Gateway { get; set; }
        public string PaymentReference { get; set; }
        public string TransactionID { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal AmountDue { get; set; }
    }
}
