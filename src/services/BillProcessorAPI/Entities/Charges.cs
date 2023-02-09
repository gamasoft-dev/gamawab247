namespace BillProcessorAPI.Entities
{
    public class BillCharge : AuditableEntity
    {
        public Guid Id { get; set; }
        public string ChannelModel { get; set; }
        public decimal MaxChargeAmount { get; set; }
        public decimal MinChargeAmount { get; set; }
        public double PercentageCharge { get; set; }
    }
}
