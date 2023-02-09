using BillProcessorAPI.Enums;

namespace BillProcessorAPI.Entities
{
    public class BillTransaction : AuditableEntity
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public EGatewayType GatewayType { get; set; }
        public ETransactionStatus Status { get; set; }
        public string SystemReference { get; set; } // Transaction reference
        public string GatewayTransactionReference { get; set; } // TrnasactionId
        public decimal AmountPaid { get; set; } // Include charges
        public decimal BillAmount { get; set; }
        public decimal PrincipalPay { get; set; } // Amount without charge
        public decimal TransactionCharge { get; set; }
        public decimal GatewayTransactionCharge { get; set; }
        public string Narration { get; set; }
        public EPaymentChannel Channel { get; set; }
        public string ResourcePIN { get; set; } // PropertyPIN
        public string ReceiptUrl { get; set; }
        public BillPayerInfo User { get; set; }
    }
}
