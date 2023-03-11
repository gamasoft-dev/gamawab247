namespace BillProcessorAPI.Dtos
{
	
	public class BillPaymentVerificationResponseDto
	{
        public string receipt { get; set; }
        public string amountpaid { get; set; }
        public string status { get; set; }
        public string statusmessage { get; set; }
    }

	public class BillPaymentVerificationRequestDto
	{
		public string WebGuid { get; set; }
		public string State { get; set; }
		public string Hash { get; set; }
		public string ClientId { get; set; }
	}
}
