namespace BillProcessorAPI.Dtos
{
	public class PaymentVerificationDto
	{
	}


	public class PaymentVerificationResponseDto
	{
		public string Receipt { get; set; }
		public string Status { get; set; }
		public string StatusMessage { get; set; }
	}

	public class PaymentVerificationRequestDto
	{
		public string WebGuid { get; set; }
		public string State { get; set; }
		public string Hash { get; set; }
		public string ClientId { get; set; }
	}
}
