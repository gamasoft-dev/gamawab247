using BillProcessorAPI.Enums;
using Domain.Enums;

namespace BillProcessorAPI.Dtos
{
    public class ReportParameters
    {
        public string PaymentGateway { get; set; }
        public string SearchType { get; set; }
    }
}
