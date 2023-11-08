using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.DashboardDtos
{
    public class TransactionDashboardStatsDto
    {
        public TransactionDashboardStatsDto()
        {
            MonthlyTransactionSummary = new HashSet<MonthlyTransactionSummaryDto>();
            YearlyTransactionSummary = new HashSet<YearlyTransactionSummaryDto>();
        }

        public WeeklyTransactionSummaryDto WeeklySummary { get; set; }
        public ICollection<MonthlyTransactionSummaryDto> MonthlyTransactionSummary { get; set; }
        public ICollection<YearlyTransactionSummaryDto> YearlyTransactionSummary { get; set; }


    }
    public class TransactionStatisticsDto
    {
        public int Year { get; set; }
        public string MonthName { get; set; }
        public decimal LowestPayment { get; set; }
        public decimal AveragePayment { get; set; }
        public decimal HighestPayment { get; set; }
    }

    public class WeeklyTransactionSummaryDto : BaseSummaryDto
    {
        public int CurrentWeek { get; set; }

    }
    public class MonthlyTransactionSummaryDto : BaseSummaryDto
    {
        public int Year { get; set; }
        public string Month { get; set; }
    }

    public class YearlyTransactionSummaryDto : BaseSummaryDto
    {
        public int Year { get; set; }
    }

    public class BaseSummaryDto
    {
        public decimal LowestPayment { get; set; }
        public decimal AveragePayment { get; set; }
        public decimal HighestPayment { get; set; }
    }
}
