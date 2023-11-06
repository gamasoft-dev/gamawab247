using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;

namespace Application.DTOs.DashboardDtos
{
    public class BaseCountDto
    {
        public int Count { get; set; }
    }
    public class GetStatisticsDto
    {
        public GetStatisticsDto()
        {
            RequestAndComplaintDataEnterPieChart = new HashSet<ChartSeries>();

            //user count
            CustomerCountsByWeek = new HashSet<WhatsappCustomerStatsByWeekDto>();
            CustomerCountsByMonth = new HashSet<WhatsappCustomerStatsByMonthDto>();
            CustomerCountsByYear = new HashSet<WhatsappCustomerStatsByYearDto>();

            //message count
            MessageLogStatByMonth = new HashSet<MessageLogStatByMonthDto>();

        }
        public int RequestCount { get; set; }
        public int ComplaintCount { get; set; }
        public ICollection<ChartSeries> RequestAndComplaintDataEnterPieChart { get; set; }
        public ICollection<WhatsappCustomerStatsByWeekDto> CustomerCountsByWeek { get; set; }
        public ICollection<WhatsappCustomerStatsByMonthDto> CustomerCountsByMonth { get; set; }
        public ICollection<WhatsappCustomerStatsByYearDto> CustomerCountsByYear { get; set; }
        public MessageLogCountDto TotalMessageCount { get; set; }
        public MessageLogStatByWeekDto MessageLogStatByWeek { get; set; }
        public ICollection<MessageLogStatByMonthDto> MessageLogStatByMonth { get; set; }


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

    public class WhatsappCustomerStatsByWeekDto : BaseCountDto
    {
        public string DayOfWeek { get; set; }
    }

    public class WhatsappCustomerStatsByMonthDto : BaseCountDto
    {
        public int Year { get; set; }
        public int MonthOfYear { get; set; }
        public string MonthName { get; set; }
    }

    public class WhatsappCustomerStatsByYearDto : BaseCountDto
    {
        public int Year { get; set; }
    }

    public class MessageLogCountDto
    {
        public int TotalMessageCount { get; set; }
    }

    public class MessageLogStatByWeekDto : BaseCountDto
    {
        public string Channel { get; set; }
    }

    public class MessageLogStatByMonthDto : BaseCountDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string Channel { get; set; }
    }

    public class MessageLogStatByYearDto : BaseCountDto
    {
        public int Year { get; set; }
        public string Channel { get; set; }
    }

}
