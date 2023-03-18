using BillProcessorAPI.Enums;

namespace BillProcessorAPI.Dtos
{
    public class PaymentVerificationResponseDto
    {
        public string Description { get; set; }
        public ETransactionResponseCodes ResponseCode { get; set; }
        public string TransactionReference { get; set; }
    }
}
