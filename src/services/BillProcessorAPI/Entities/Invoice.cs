namespace BillProcessorAPI.Entities
{
    public class Invoice : AuditableEntity
    {
        public Guid Id { get; set; }
        public decimal AmountDue { get; set; }
        public string AgencyCode { get; set; }
        public string PayerName { get; set; }
        public string RevenueCode { get; set; }
        public string OraAgencyRev { get; set; }
        public string State { get; set; }
        public string Pid { get; set; }
        public string AcctCloseDate { get; set; }
        public string PaymentFlag { get; set; }
        public string CbnCode { get; set; }
        public string AgencyName { get; set; }
        public string RevName { get; set; }
        public string AccountInfoResponseData { get; set; }
        public string AccountInfoRequestData { get; set; }
    }
}
