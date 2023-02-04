using BillProcessorAPI.Enums;

namespace BillProcessorAPI.Entities
{
    public class BillTransaction : AuditableEntity
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public EGatewayType GatewayType { get; set; }
        public ETransactionStatus Status { get; set; }
        public string SystemReference { get; set; }
        public string GatewayTransactionReference { get; set; }
        public decimal Amount { get; set; }
        public decimal TransactionCharge { get; set; }
        public string Narration { get; set; }
        public BillPayerInfo User { get; set; }
    }
}
