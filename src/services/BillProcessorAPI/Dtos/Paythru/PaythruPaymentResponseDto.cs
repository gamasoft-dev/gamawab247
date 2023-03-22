namespace BillProcessorAPI.Dtos.Paythru
{
    public class PaythruPaymentResponseDto
    {
        public string successIndicator { get; set; }
        public string payLink { get; set; }
        public BankTransferInstruction bankTransferInstruction { get; set; }
        public object splitPayResult { get; set; }
        public object emvCode { get; set; }
        public int code { get; set; }
        public decimal systemCharge { get; set; }
        public string message { get; set; }
        public object data { get; set; }
        public bool successful { get; set; }

    }

    public class BankTransferInstruction
    {
        public string accountNumber { get; set; }
        public string accountName { get; set; }
        public string bankName { get; set; }
        public double transactionAmount { get; set; }
        public double charges { get; set; }
        public double total { get; set; }
    }
}
