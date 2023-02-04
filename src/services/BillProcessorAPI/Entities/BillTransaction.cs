using BillProcessorAPI.Enums;

namespace BillProcessorAPI.Entities
{
    public class BillTransaction : AuditableEntity
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public EGatewayType GatewayType { get; set; }
        public ETransactionStatus Status { get; set; }
        public Guid Reference { get; set; }
        public decimal Amount { get; set; }
        public decimal TransactionCharge { get; set; }
        public User User { get; set; }
    }
}
