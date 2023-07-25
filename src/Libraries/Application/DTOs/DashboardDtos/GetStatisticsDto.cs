using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.DashboardDtos
{
    public class GetStatisticsDto
    {
        public GetStatisticsDto()
        {
            RequestAndComplaintDataEnterPieChart = new HashSet<ChartSeries>();
            CustomerCountsPerDay = new HashSet<WhatsappCustomerStatsDto>();

        }
        public int RequestCount { get; set; }
        public int ComplaintCount { get; set; }
        public ICollection<ChartSeries> RequestAndComplaintDataEnterPieChart { get; set; }
        public ICollection<WhatsappCustomerStatsDto> CustomerCountsPerDay { get; set; }


    }

    public class ChartSeries
    {
        public string Name { get; set; }
        public ResolutionStatusDto ResolutionChartData { get; set; }
    }

    public class ResolutionStatusDto
    {
        public int PendingCount { get; set; }
        public int EscalatedCount { get; set; }
        public int ProcessingCount { get; set; }
        public int CompletedCount { get; set; }

    }

    public class WhatsappCustomerStatsDto
    {
        public string DayOfWeek { get; set; }
        public int Count { get; set; }
    }

}
