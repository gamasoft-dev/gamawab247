using BillProcessorAPI.Entities;
using BillProcessorAPI.Enums;

namespace BillProcessorAPI.Dtos
{
	public record CollectionReportDto
	{
        public string PayerName { get; set; }
        public string BillNumber { get; set; }
		public string Pid { get; set; }
        public EGatewayType GatewayType { get; set; }
        public string Platform { get; set; }
		public decimal AmountDue { get; set; }
		public decimal AmountPaid { get; set; }
		public decimal Principal { get; set; }
		public decimal TransactionFee { get; set; }
		public string DateCompleted { get; set; }


		public static implicit operator CollectionReportDto(BillTransaction model)
		{
			return model is null ? null : new CollectionReportDto
			{
                PayerName = model.PayerName,
                BillNumber = model.BillNumber,
                Pid = model.Pid,
                GatewayType = model.GatewayType,
				Platform = "N/A",
                AmountDue = model.AmountDue,
				AmountPaid = model.AmountPaid,
				Principal = model.PrinciPalAmount,
				TransactionFee = model.TransactionCharge,
                DateCompleted = model.DateCompleted
			};
		}
	}

	
}
