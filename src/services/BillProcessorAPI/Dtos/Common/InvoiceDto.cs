using BillProcessorAPI.Entities;
using BillProcessorAPI.Enums;

namespace BillProcessorAPI.Dtos.Common
{
    public class InvoiceDto:InvoiceDetailsDto
    {
        public Guid Id { get; set; }
       
    }

    public class InvoiceDetailsDto
    {
        public Guid BillTransactionId { get; set; }
        public Guid? ReceiptId { get; set; }
        public string PayerName { get; set; }
        public string BillNumber { get; set; }
        public string Pid { get; set; }
        public EGatewayType GatewayType { get; set; }
        public string TransactionReference { get; set; }
        public string GatewayTransactionReference { get; set; }
        public string RevName { get; set; }
        public string PhoneNumber { get; set; }
        public string DueDate { get; set; }
        public decimal AmountDue { get; set; }
        public decimal AmountPaid { get; set; } // Include charges
        public decimal TransactionCharge { get; set; }
        public decimal GatewayTransactionCharge { get; set; }
        public string ReceiptUrl { get; set; }
        public BillTransaction BillTransaction { get; set; }
        public Receipt Receipt { get; set; }
    }
}
