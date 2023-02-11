using BillProcessorAPI.Enums;

namespace BillProcessorAPI.Entities
{
    public class BillTransaction : AuditableEntity
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public EGatewayType GatewayType { get; set; }
        public string Status { get; set; }
        public string SystemReference { get; set; }
        public string GatewayTransactionReference { get; set; }
        public decimal AmountPaid { get; set; } // Include charges
        public decimal BillAmount { get; set; }
        public decimal PrinciPalAmount { get; set; } // Amount without charge
        public decimal TransactionCharge { get; set; }
        public string Narration { get; set; }
        public string Channel { get; set; }
        public string ResourcePIN { get; set; } // PropertyPIN
        public string ReceiptUrl { get; set; }
        public string Receipt { get; set; }
        public string StatusMessage { get; set; }
        public string PaymentInfoRequestData { get; set; }
		public string PaymentInfoResponseData { get; set; }
		public BillPayerInfo User { get; set; }
    }
}
