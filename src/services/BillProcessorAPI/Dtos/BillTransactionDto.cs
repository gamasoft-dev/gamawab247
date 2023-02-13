namespace BillProcessorAPI.Dtos
{
	
	public class BillPaymentVerificationResponseDto
	{
		public string Receipt { get; set; }
		public string Status { get; set; }
		public string StatusMessage { get; set; }
	}

	public class BillPaymentVerificationRequestDto
	{
		public string WebGuid { get; set; }
		public string State { get; set; }
		public string Hash { get; set; }
		public string ClientId { get; set; }
	}
}
