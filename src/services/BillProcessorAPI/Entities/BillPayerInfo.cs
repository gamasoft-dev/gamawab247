namespace BillProcessorAPI.Entities
{
    public class BillPayerInfo : AuditableEntity
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string BillNumber { get; set; }
        public string PropertyNumber { get; set; }
        public string Purpose { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? DueDate { get; set; }
        public ICollection<BillTransaction> BillTransactions { get; set; } = new List<BillTransaction>();
    }
}
