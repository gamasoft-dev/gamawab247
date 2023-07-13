using BillProcessorAPI.Enums;

namespace BillProcessorAPI.Entities.ValueObjects
{
    public class SearchQueryParams
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string PaymentGateway { get; set; }
        public string SearchType { get; set; }
        public string SearchText { get; set; }

    }
}
