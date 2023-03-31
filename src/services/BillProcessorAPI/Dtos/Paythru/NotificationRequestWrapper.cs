namespace BillProcessorAPI.Dtos.Paythru
{
    public class NotificationRequestWrapper
    {
        public int NotificationType { get; set; }
        public TransactionNotificationDetail TransactionDetails { get; set; }
    }

    public class TransactionNotificationDetail
    {
        public string ProductId { get; set; }

        public string DateOfTransaction { get; set; } //=> TransactionDate.ToString("yyyy-MM-ddThh:mmZ");
        public string MerchantReference { get; set; }
        /// <summary>
        /// Identification of paying customer
        /// </summary>
        public int? CustomerId { get; set; }
        public string FiName { get; set; }
        public string PaymentMethod { get; set; }

        public string PayThruReference { get; set; }
        /// <summary>
        /// This is the payment reference returned by the FI
        /// </summary>
        public string PaymentReference { get; set; }
        public string BankReference { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseDescription { get; set; }

        public string DateCompleted { get; set; }
        public string Naration { get; set; }
        public string Currency { get; set; }
        public string UssdReferenceCode { get; set; }

        public string Status { get; set; }

        public decimal Amount { get; set; }

        public decimal Commission { get; set; }
        public decimal ResidualAmount { get; set; }
        public string CustomerName { get; set; }

        /// <summary>
        /// This should be compared to the success indicator parameter returned when the transaction request was made for single Open or Single Fixed transactions
        /// </summary>
        public string ResultCode { get; set; }

        /// <summary>
        /// SHA512 hash value of Amount+Commission+ResidualAmount+Client's secret
        /// </summary>
        public string Hash { get; set; }
        public string ReceiptUrl { get; set; }
    }
}

