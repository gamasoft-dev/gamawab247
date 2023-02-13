namespace BillProcessorAPI.Dtos
{
	public class CollectionReport
	{
		public string BillCode { get; set; }
		public string PropertyPin { get; set; }
		public string Gateway { get; set; }
		public string Platform { get; set; }
		public string BillAmount { get; set; }
		public string AmountPaid { get; set; }
		public string Principal { get; set; }
		public string TransactionFee { get; set; }
		public string Date { get; set; }
	}
}
