using System;
namespace ApiCustomization.ABC.Dtos
{
    
	public record BillReferenceResponse
	{
        public string amountDue { get; set; }
        public string status { get; set; }
        public string creditAccount { get; set; }
        public string payerName { get; set; }
        public string agencyCode { get; set; }
        public string revenueCode { get; set; }
        public string oraAgencyRev { get; set; }
        public string state { get; set; }
        public string statusMessage { get; set; }
        public string pid { get; set; }
        public string currency { get; set; }
        public string acctCloseDate { get; set; }
        public string readOnly { get; set; }
        public string minAmount { get; set; }
        public string maxAmount { get; set; }
        public string paymentFlag { get; set; }
        public string cbnCode { get; set; }
        public string agencyName { get; set; }
        public string revName { get; set; }
    }
}

