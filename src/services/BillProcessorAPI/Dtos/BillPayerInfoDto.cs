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
        [JsonProperty("amount_due")]
        public string AmountDue { get; set; }
        [JsonProperty("status")]
        public string status { get; set; }
        [JsonProperty("credit_account")]
        public string CreditAccount { get; set; }
        [JsonProperty("payer_name")]
        public string PayerName { get; set; }
        [JsonProperty("agency_code")]
        public string AgencyCode { get; set; }
        [JsonProperty("revenue_code")]
        public string RevenueCode { get; set; }
        [JsonProperty("ora_agency_rev")]
        public string OraAgencyRev { get; set; }
        [JsonProperty("state")]
        public string State { get; set; }
        [JsonProperty("statusMessage")]
        public string statusMessage { get; set; }
        [JsonProperty("p_id")]
        public string Pid { get; set; }
        [JsonProperty("currency")]
        public string Currency { get; set; }
        [JsonProperty("acct_close_date")]
        public string AcctCloseDate { get; set; }
        [JsonProperty("read_only")]
        public string ReadOnly { get; set; }
        [JsonProperty("min_amount")]
        public string MinAmount { get; set; }
        [JsonProperty("max_amount")]
        public string MaxAmount { get; set; }
        [JsonProperty("payment_flag")]
        public string PaymentFlag { get; set; }
        [JsonProperty("cbn_code")]
        public string CbnCode { get; set; }
        [JsonProperty("agency_name")]
        public string AgencyName { get; set; }
        [JsonProperty("rev_name")]
        public string RevName { get; set; }
        [JsonProperty("current_date")]
        public DateTime CurrentDate { get; set; }
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
