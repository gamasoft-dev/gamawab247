namespace BillProcessorAPI.Entities
{
    public class BillPayerInfo : AuditableEntity
    {
        public Guid Id { get; set; }
        public string PayerName { get; set; }
        public string billCode { get; set; }
        public string PhoneNumber { get; set; }
        public decimal AmountDue { get; set; }
        public string Status { get; set; }
        public string CreditAccount { get; set; }
        public string AgencyCode { get; set; }
        public string RevenueCode { get; set; }
        public string OraAgencyRev { get; set; }
        public string State { get; set; }
        public string StatusMessage { get; set; }
        public string Pid { get; set; }
        public string Currency { get; set; }
        public string AcctCloseDate { get; set; }
        public string ReadOnly { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public string PaymentFlag { get; set; }
        public string CbnCode { get; set; }
        public string AgencyName { get; set; }
        public string RevName { get; set; }
        public string PhoneNumber { get; set; }
        public string AccountInfoResponseData { get; set; }
        public string AccountInfoRequestData { get; set; }

        public ICollection<BillTransaction> BillTransactions { get; set; } = new List<BillTransaction>();
    }
}
