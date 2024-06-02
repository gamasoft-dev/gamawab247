using BillProcessorAPI.Dtos;
using BillProcessorAPI.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillProcessorAPI.Entities
{
    public class BillTransaction : AuditableEntity
    {
        public Guid Id { get; set; }
        public string PayerName { get; set; }
        public string BillNumber { get; set; }
        public string Pid { get; set; }
        public string RevName { get; set; }
        public string PhoneNumber { get; set; }
        public string DueDate { get; set; }
        public string DateCompleted { get; set; }
        public EGatewayType GatewayType { get; set; }
        public string Status { get; set; }
        public string TransactionReference { get; set; }
        public string GatewayTransactionReference { get; set; }
        public string PaymentReference { get; set; }
        public string FiName { get; set; }
        public decimal Amount { get; set; }
        public decimal AmountDue { get; set; }
        public decimal AmountPaid { get; set; } // Include charges
        //public decimal BillAmount { get; set; }
        public decimal PrinciPalAmount { get; set; } // Amount without charge
        public decimal TransactionCharge { get; set; }
        public decimal GatewayTransactionCharge { get; set; }
        public string Narration { get; set; }
        public string Channel { get; set; }
        public string ResourcePIN { get; set; } // PropertyPIN
        public string PaymentUrl { get; set; }
        public string ReceiptUrl { get; set; }
        public bool isReceiptSent { get; set; }
        public string StatusMessage { get; set; }
        public string Hash { get; set; }
        public string SuccessIndicator { get; set; }
        public string PaymentInfoRequestData { get; set; }
		public string PaymentInfoResponseData { get; set; }
		public string NotificationResponseData { get; set; }
        public Guid? BillPayerInfoId { get; set; }
        public BillPayerInfo BillPayerInfo { get; set; }
        public string ErrorMessage { get; set; }
        [NotMapped]
        public string Email { get; set; }
        //public Guid? InvoiceId  { get; set; }
        //public Invoice Invoice { get; set; }
    }
}
