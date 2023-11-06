using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3.Model;
using Application.DTOs;
using Application.DTOs.DashboardDtos;
using Application.DTOs.RequestAndComplaintDtos;
using Application.Helpers;
using Application.Services.Interfaces.Dashboard;
using Domain.Common;
using Domain.Entities;
using Domain.Entities.Identities;
using Domain.Entities.RequestAndComplaints;
using Domain.Enums;
using Infrastructure.Http;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using static Amazon.S3.Util.S3EventNotification;


namespace Application.Services.Implementations.Dashboard
{
    public class DashboardStatisticsService : IDashboardStatisticsService
    {
        private readonly IRepository<RequestAndComplaint> _requestAndComplaintRepo;
        private readonly IWhatsappUserRepository _whatsappUserRepo;
        private readonly IMessageLogRepository _messageLogRepo;
        private readonly IHttpService _httpService;
        private readonly TransactionOptions _transactionOptions;

        public DashboardStatisticsService(IRepository<RequestAndComplaint> requestAndComplaintRepo, IWhatsappUserRepository whatsappUserRepo, IMessageLogRepository messageLogRepo, IHttpService httpService, IOptions<TransactionOptions> transactionOptions)
        {
            _requestAndComplaintRepo = requestAndComplaintRepo;
            _whatsappUserRepo = whatsappUserRepo;
            _messageLogRepo = messageLogRepo;
            _httpService = httpService;
            _transactionOptions = transactionOptions.Value;
        }

        //Get Request and complaint statistics
        public async Task<SuccessResponse<GetStatisticsDto>> GetStatistics()
        {

            // Calculate the start of the week (Sunday)
            var currentDate = DateTime.UtcNow.AddHours(1);
            int diff = currentDate.DayOfWeek - DayOfWeek.Sunday;
            if (diff < 0)
                diff += 7; // Handle negative difference when the current date is Sunday
            var weekStartDate = currentDate.Date.AddDays(-diff);
            // Calculate the end of the week (Saturday)
            var weekEndDate = weekStartDate.AddDays(6);

            //Month start date and end date
            var monthStartDate = new DateTime(currentDate.Year, 1, 1).ToUniversalTime().AddHours(1); // First day of the current year
            var monthEndDate = new DateTime(currentDate.Year, 12, DateTime.DaysInMonth(currentDate.Year, 12)).ToUniversalTime(); // Last day of the current year

            var getAllrequestAndComplain = await _requestAndComplaintRepo.Query(x => x.Type == ERequestComplaintType.Request || x.Type == ERequestComplaintType.Complaint)
                .Select(u => new RequestAndComplaintStat
                {
                    Id = u.Id,
                    ResolutionStatus = u.ResolutionStatus,
                    Type = u.Type
                }).ToListAsync();
            var rQ = getAllrequestAndComplain.GroupBy(x => x.Type);

            var request = rQ.Where(group => group.Key == ERequestComplaintType.Request)
            .SelectMany(group => group.ToList())
            .ToList();

            var complaint = rQ.Where(group => group.Key == ERequestComplaintType.Complaint)
            .SelectMany(group => group.ToList())
            .ToList();

            try
            {
            var stats = new GetStatisticsDto
            {
                    RequestCount = request.Count,
                    ComplaintCount = complaint.Count
            };

                var requestChartData = GetResolutionStatus(request);
                var complaintChartData = GetResolutionStatus(complaint);

                ////request and complaint stats
            stats.RequestAndComplaintDataEnterPieChart.Add(new ChartSeries { Name = "Request", ResolutionChartData = requestChartData });
            stats.RequestAndComplaintDataEnterPieChart.Add(new ChartSeries { Name = "Complaint", ResolutionChartData = complaintChartData });

            //user stats
                stats.CustomerCountsByWeek = await GetWhatsappCustomerCountByWeek(weekStartDate, weekEndDate);
            stats.CustomerCountsByMonth = await GetWhatsappCustomerCountByMonth(monthStartDate, monthEndDate);
            stats.CustomerCountsByYear = await GetWhatsappCustomerCountByYear();

            //message stats
            stats.TotalMessageCount = await TotalWhatsappMessageCount();
            stats.MessageLogStatByWeek = await WhatsappMessageStatsWeekly(weekStartDate, weekEndDate);
            stats.MessageLogStatByMonth = await WhatsappMessageStatsForMonthsOfYear();
                stats.MessageLogStatByYear = await WhatsappMessageStatsForYears();

            //Get user statistics
                stats.TransactionStats = await GetTransactionSummary();


            return new SuccessResponse<GetStatisticsDto>
            {
                    Message = "success",
                Data = stats
            };
        }
            catch (Exception)
        {
                throw new RestException(HttpStatusCode.RequestTimeout, "An error occured");
        }

        }

        //Customer Statistics
        #region WhatsAppUsers Count

        private async Task<List<WhatsappCustomerStatsByWeekDto>> GetWhatsappCustomerCountByWeek(DateTime weekStartDate, DateTime weekEndDate)
        {

            if (weekStartDate > weekEndDate)
            {
                throw new RestException(HttpStatusCode.BadRequest, "Invalid date range. The start date must be before or equal to the end date.");
            }

            // Group users by the day of the week and get the counts for each day
            var userCountsPerWeek = await _whatsappUserRepo.Query(u => u.CreatedAt >= weekStartDate && u.CreatedAt <= weekEndDate)
                .GroupBy(u => u.CreatedAt.DayOfWeek)
                .Select(group => new { DayOfWeek = group.Key, Count = group.Count() })
                .ToListAsync();

            // Create a dictionary to store user counts for each day of the week
            var userCountPerWeekDictionary = userCountsPerWeek.ToDictionary(
                item => item.DayOfWeek,
                item => new WhatsappCustomerStatsByWeekDto { DayOfWeek = item.DayOfWeek.ToString(), Count = item.Count }
            );

    
            // Convert the dictionary values back to a list
            var userCountPerWeek = userCountPerWeekDictionary.Values.ToList();

            return userCountPerWeek;
        }

        private async Task<List<WhatsappCustomerStatsByMonthDto>> GetWhatsappCustomerCountByMonth(DateTime startDate, DateTime endDate)
        {
    
            if (startDate > endDate)
            {
                throw new RestException(HttpStatusCode.BadRequest, "Invalid date range. The start date must be before or equal to the end date.");
            }

            // Group users by the year and month and get the counts for each month
            var userCountsPerMonth = await _whatsappUserRepo.Query(u => u.CreatedAt >= startDate && u.CreatedAt <= endDate)
                .GroupBy(u => new { u.CreatedAt.Year, u.CreatedAt.Month })
                .Select(group => new { Year = group.Key.Year, MonthOfYear = group.Key.Month, Count = group.Count() })
                .ToListAsync();

            // Create a dictionary to store user counts for each year-month combination
            var userCountPerMonthDictionary = userCountsPerMonth.ToDictionary(
                item => (item.Year, item.MonthOfYear),
                item => new WhatsappCustomerStatsByMonthDto
            {
                    Year = item.Year,
                    MonthOfYear = item.MonthOfYear,
                    MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.MonthOfYear),
                    Count = item.Count
                }
            );

            // Convert the dictionary values back to a list
            var userCountPerMonth = userCountPerMonthDictionary.Values.OrderBy(x => x.MonthOfYear).ToList();

            return userCountPerMonth;
        }

        private async Task<List<WhatsappCustomerStatsByYearDto>> GetWhatsappCustomerCountByYear()
        {
            var currentYear = 2023;
            var startDate = new DateTime(currentYear, 1, 1).AddHours(1).ToUniversalTime(); // First day of the current year

            // Group users by the year and get the counts for each year
            var userCountsPerYear = await _whatsappUserRepo.Query(u => u.CreatedAt >= startDate)
                .GroupBy(u => u.CreatedAt.Year)
                .Select(group => new { Year = group.Key, Count = group.Count() })
                .ToListAsync();

            // Initialize the dictionary with user counts for each year
            var userCountPerYearDictionary = userCountsPerYear.ToDictionary(
                item => item.Year,
                item => new WhatsappCustomerStatsByYearDto { Year = item.Year, Count = item.Count }
            );

            // Convert the dictionary values back to a list
            var userCountPerYear = userCountPerYearDictionary.Values.OrderBy(x => x.Year).ToList();

            return userCountPerYear;
        }

        #endregion


        //message statistics 
        #region Message count statistics
        private async Task<MessageLogCountDto> TotalWhatsappMessageCount()
        {
            var totalMessage = await _messageLogRepo.MessageLogCountAsync();
            var totalCount = new MessageLogCountDto
            {
                TotalMessageCount = totalMessage
            };
            return totalCount;
        }

        private async Task<MessageLogStatByWeekDto> WhatsappMessageStatsWeekly(DateTime weekStartDate, DateTime weekEndDate)
        {

            if (weekStartDate > weekEndDate)
            {
                throw new RestException(HttpStatusCode.BadRequest, "Invalid date range. The start date must be before or equal to the end date.");
            }

            // Count user for the current week
            var userCountsPerWeek = await _messageLogRepo.Query(u => u.CreatedAt >= weekStartDate && u.CreatedAt <= weekEndDate).CountAsync();

            var userWeeklyCount = new MessageLogStatByWeekDto
            {
                Count = userCountsPerWeek,
                Channel = "WhatsApp"
            };
            return userWeeklyCount; 
        }


        private async Task<List<MessageLogStatByMonthDto>> WhatsappMessageStatsForMonthsOfYear()
        {
            // Get the current year
            var currentYear = 2023;

            // Get the earliest record date from the database
            var earliestRecordDate = _messageLogRepo.Query(u => u.CreatedAt != DateTime.MinValue).Min(u => u.CreatedAt);

            // Determine the starting month from the earliest record date
            var startingMonth = earliestRecordDate.ToUniversalTime().Month;

            // Calculate the start date of the first month
            var firstMonthStartDate = new DateTime(currentYear, startingMonth, 1).ToUniversalTime();

            // Calculate the end date of the last month
            var lastMonthEndDate = new DateTime(currentYear, 12, DateTime.DaysInMonth(currentYear, 12)).ToUniversalTime();

            // Fetch all message counts for the year in a single database call
            var monthlyCounts = await _messageLogRepo.Query(u => u.CreatedAt >= firstMonthStartDate && u.CreatedAt <= lastMonthEndDate)
                .GroupBy(u => new { u.CreatedAt.Month })
                .Select(group => new MessageLogStatByMonthDto
                {
                    MonthOfYear = group.Key.Month,
                    MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(group.Key.Month),
                    Year = currentYear,
                    Count = group.Count(),
                    Channel = "WhatsApp"
                })
                .ToListAsync();

            // Order the monthly counts by month
            monthlyCounts = monthlyCounts.OrderBy(dto => dto.MonthOfYear).ToList();

            return monthlyCounts;
        }


        private async Task<List<MessageLogStatByYearDto>> WhatsappMessageStatsForYears()
        {
            // Get the current year
            var currentYear = DateTime.UtcNow.Year;

            // Get the earliest record year from the database
            var earliestRecordYear = _messageLogRepo.Query(u => u.CreatedAt != DateTime.MinValue).Min(u => u.CreatedAt.Year);

            // Fetch all message counts for the years in a single database call
            var yearlyCounts = await _messageLogRepo.Query(u => u.CreatedAt.Year >= earliestRecordYear && u.CreatedAt.Year <= currentYear)
                .GroupBy(u => new MessageLogStatByYearDto { Year = u.CreatedAt.Year })
                .Select(group => new MessageLogStatByYearDto
                {
                    Year = group.Key.Year,
                    Count = group.Count(),
                    Channel = "WhatsApp"
                })
                .ToListAsync();

            return yearlyCounts;
        }

        #endregion

        //Transaction statistics
        private async Task<TransactionDashboardStatsDto> GetTransactionSummary()
            {
            var url = _transactionOptions.TransactionSummaryUrl;

            IDictionary<string, string> param = new Dictionary<string, string>();
            param.Add(key: _transactionOptions.TrxHeaderKey, value: _transactionOptions.TrxHeaderValue);
            var headerParam = new RequestHeader(param);

            var getTransactionSummary = await _httpService.Get<TransactionDashboardStatsDto>(url, headerParam);
            if (getTransactionSummary.Data == null)
                throw new RestException(HttpStatusCode.BadRequest, "An error occured while processing your request");

            var result = new TransactionDashboardStatsDto
            {
                WeeklySummary = getTransactionSummary.Data.WeeklySummary,
                MonthlyTransactionSummary = getTransactionSummary.Data.MonthlyTransactionSummary,
                YearlyTransactionSummary = getTransactionSummary.Data.YearlyTransactionSummary
            };
            return result;
        }

        private static ResolutionStatusDto GetResolutionStatus(List<RequestAndComplaintStat> data)
        {
            var resolutionStatusData = new ResolutionStatusDto
            {
                PendingCount = data.Where(x => x.ResolutionStatus == EResolutionStatus.Pending.ToString()).Count(),
                EscalatedCount = data.Where(x => x.ResolutionStatus == EResolutionStatus.Escalated.ToString()).Count(),
                ProcessingCount = data.Where(x => x.ResolutionStatus == EResolutionStatus.Processing.ToString()).Count(),
                CompletedCount = data.Where(x => x.ResolutionStatus == EResolutionStatus.Completed.ToString()).Count(),
            };
            return resolutionStatusData;
        }

       



    }
}
