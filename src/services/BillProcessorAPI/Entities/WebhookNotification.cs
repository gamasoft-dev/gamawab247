namespace BillProcessorAPI.Entities
{
    public class WebhookNotification : AuditableEntity
    {
        public Guid Id { get; set; }

        public string WebGuid { get; set; }

        public string ResponseCode { get; set; }

        public string ResponseDesc { get; set; }

        public string ReceiptNumber { get; set; }

        public string State { get; set; }

        public string Status { get; set; }

        public string TransID { get; set; }

        public string TransCode { get; set; }

        public string StatusMessage { get; set; }

        public string PropertyAddress { get; set; }


        public string TransactionReference { get; set; }

        // this is a dynamic type and it should handle both int and or strings
        public int PaymentRef { get; set; }
        public string Data { get; set; }
        public string GatewayType { get; set; }
        public string Remark { get; set; }
    }
}
