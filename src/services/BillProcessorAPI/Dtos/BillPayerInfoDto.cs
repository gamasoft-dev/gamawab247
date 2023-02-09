namespace BillProcessorAPI.Dtos
{
	public class BillPayerInfoDto
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
	}

	public class BillReferenceRequestDto
	{
		public string BillReference { get; set; }
	}

	public class BillReferenceResponseDto<T> where T : class
	{
		public string Status { get; set; }
		public string Message { get; set; }
		public T Data { get; set; }
	}
}
