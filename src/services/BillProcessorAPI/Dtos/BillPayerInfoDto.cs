using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace BillProcessorAPI.Dtos
{
	public record BillPayerInfoDto
	{
		public decimal AmountDue { get; set; }
		public string Status { get; set; }
		public string CreditAccount { get; set; }
		public string PayerName { get; set; }
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
	}

	public record BillReferenceResponseDto
    {
        public string AmountDue { get; set; }
        public string Status { get; set; }
        public string CreditAccount { get; set; }
        public string PayerName { get; set; }
        public string AgencyCode { get; set; }
        public string RevenueCode { get; set; }
        public string OraAgencyRev { get; set; }
        public string State { get; set; }
        public string StatusMessage { get; set; }
        public string Pid { get; set; }
        public string Currency { get; set; }
        public string AcctCloseDate { get; set; }
        public string ReadOnly { get; set; }
        public string MinAmount { get; set; }
        public string MaxAmount { get; set; }
        public string PaymentFlag { get; set; }
        public string CbnCode { get; set; }
        public string AgencyName { get; set; }
        public string RevName { get; set; }
        public string AccountInfoResponseData { get; set; }
        public string AccountInfoRequestData { get; set; }
    }

	public record BillRequestDto
	{
        [JsonPropertyName("billPaymentCode")]
		public string BillPaymentCode { get; set; }
    }

    public record AbcRequestPayload
    {
        public string Webguid { get; set; }
        public string State { get; set; }
        public string Hash { get; set; }
        public string ClientId { get; set; }
        public string Type { get; set; }
    }

}
