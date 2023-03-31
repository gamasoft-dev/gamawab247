using BillProcessorAPI.Entities;

namespace BillProcessorAPI.Dtos.Common
{
    public class ReceiptDto:ReceiptDetailsDto
    {
        public Guid Id { get; set; }
        
    }


    public class ReceiptDetailsDto
    {
        public Guid TransactionId { get; set; }
        public string TransactionDate { get; set; }
        public string GateWay { get; set; }
        public string PaymentRef { get; set; }
        public string GatewayTransactionReference { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal AmountDue { get; set; }
        public string ReceiptUrl { get; set; }
    }
}
