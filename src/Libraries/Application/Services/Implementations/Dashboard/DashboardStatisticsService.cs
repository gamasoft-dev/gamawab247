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
using Application.Helpers;
using Application.Services.Interfaces.Dashboard;
using Domain.Entities;
using Domain.Entities.Identities;
using Domain.Entities.RequestAndComplaints;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using static Amazon.S3.Util.S3EventNotification;

namespace Application.Services.Implementations.Dashboard
{
    public class DashboardStatisticsService : IDashboardStatisticsService
    {
        private readonly IRepository<RequestAndComplaint> _requestAndComplaintRepo;
        private readonly IWhatsappUserRepository _whatsappUserRepo;
        private readonly IMessageLogRepository _messageLogRepo;

        public DashboardStatisticsService(IRepository<RequestAndComplaint> requestAndComplaintRepo, IWhatsappUserRepository whatsappUserRepo, IMessageLogRepository messageLogRepo)
        {
            _requestAndComplaintRepo = requestAndComplaintRepo;
            _whatsappUserRepo = whatsappUserRepo;
            _messageLogRepo = messageLogRepo;
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


            var requestsAndComplaints = await _requestAndComplaintRepo.GetAllAsync();
            var requests = requestsAndComplaints.Where(x => x.Type == ERequestComplaintType.Request);
            var complaints = requestsAndComplaints.Where(x => x.Type == ERequestComplaintType.Complaint);

            var stats = new GetStatisticsDto
            {
                RequestCount = requests.Count(),
                ComplaintCount = complaints.Count()
            };

            var requestChartData = GetResolutionStatus(requests);
            var complaintChartData = GetResolutionStatus(complaints);

            //request and complaint stats
            stats.RequestAndComplaintDataEnterPieChart.Add(new ChartSeries { Name = "Request", ResolutionChartData = requestChartData });
            stats.RequestAndComplaintDataEnterPieChart.Add(new ChartSeries { Name = "Complaint", ResolutionChartData = complaintChartData });

            //user stats
            stats.CustomerCountsByWeek = await GetWhatsappCustomerCountByWeek(weekStartDate,weekEndDate);
            stats.CustomerCountsByMonth = await GetWhatsappCustomerCountByMonth(monthStartDate, monthEndDate);
            stats.CustomerCountsByYear = await GetWhatsappCustomerCountByYear();

            //message stats
            stats.TotalMessageCount = await TotalWhatsappMessageCount();
            stats.MessageLogStatByWeek = await WhatsappMessageStatsWeekly(weekStartDate, weekEndDate);
            stats.MessageLogStatByMonth = await WhatsappMessageStatsForMonthsOfYear();

            //Get user statistics


            return new SuccessResponse<GetStatisticsDto>
            {
                Data = stats
            };
        }

        private static ResolutionStatusDto GetResolutionStatus(IEnumerable<RequestAndComplaint> data)
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


        //Customer Statistics
        #region WhatsAppUsers Count

        private async Task<List<WhatsappCustomerStatsByWeekDto>> GetWhatsappCustomerCountByWeek(DateTime weekStartDate, DateTime weekEndDate)
        {
            if (weekStartDate > weekEndDate)
            {
                throw new RestException(HttpStatusCode.BadRequest, "Invalid date range. The start date must be before or equal to the end date.");
            }

            // Group users by the day of the week and get the counts for each day
            var userCountsPerWeek = await _whatsappUserRepo.Query(u => u.CreatedAt >= weekStartDate && u.CreatedAt <= weekEndDate).ToListAsync();

            // Initialize the user counts for each day to 0
            var userCountPerWeek = await InitializeCountStatByWeek<WhatsappCustomerStatsByWeekDto>();

            // Add users to the list by days
            foreach (var user in userCountsPerWeek)
            {
                var dayOfWeek = user.CreatedAt.DayOfWeek.ToString();
                var userCountDto = userCountPerWeek.Find(dto => dto.DayOfWeek == dayOfWeek);
                if (userCountDto != null)
                {
                    userCountDto.Count++;
                }
            }
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
                .Select(group => new WhatsappCustomerStatsByMonthDto  { Year = group.Key.Year, MonthOfYear = group.Key.Month, Count = group.Count() })
                .ToListAsync();

            // Initialize the user counts for each month to 0
            var userCountPerMonth = await InitializeCountStatByMonth<WhatsappCustomerStatsByMonthDto>();

            // Add users to the list by months
            foreach (var userCount in userCountsPerMonth)
            {
                var userCountDto = userCountPerMonth.Find(dto => dto.Year == userCount.Year && dto.MonthOfYear == userCount.MonthOfYear);
                if (userCountDto != null)
                {
                    userCountDto.Count = userCount.Count;
                }
            }
            return userCountPerMonth;
        }

        private async Task<List<WhatsappCustomerStatsByYearDto>> GetWhatsappCustomerCountByYear()
        {
            var currentYear = 2023;
            var startDate = new DateTime(currentYear, 1, 1).ToUniversalTime().AddHours(1); // First day of the current year

            // Group users by the year and get the counts for each year
            var userCountsPerYear = await _whatsappUserRepo.Query(u => u.CreatedAt >= startDate)
                .GroupBy(u => u.CreatedAt.Year)
                .Select(group => new WhatsappCustomerStatsByYearDto { Year = group.Key, Count = group.Count() })
                .ToListAsync();

            // Initialize the user counts for each year to 0
            var userCountPerYear = await InitializeCountStatByYear<WhatsappCustomerStatsByYearDto>();

            // Add users to the list by years
            foreach (var userCount in userCountsPerYear)
            {
                var userCountDto = userCountPerYear.Find(dto => dto.Year == userCount.Year);
                if (userCountDto != null)
                {
                    userCountDto.Count = userCount.Count;
                }
            }
            return userCountPerYear;
        }
        #endregion


        //message statistics 
        #region Message count statistics
        private async Task<MessageLogCountDto> TotalWhatsappMessageCount()
        {
            var totalMessage =  await _messageLogRepo.GetAllAsync();
            var totalCount = new MessageLogCountDto
            {
                TotalMessageCount = totalMessage.Count()
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
            var earliestRecordDate = await _messageLogRepo.Query(u => u.CreatedAt != DateTime.MinValue).MinAsync(u => u.CreatedAt);

            // Determine the starting month from the earliest record date
            var startingMonth = earliestRecordDate.ToUniversalTime().Month;

            // Initialize a list to store the monthly counts
            var monthlyCounts = new List<MessageLogStatByMonthDto>();

            for (int month = startingMonth; month <= 12; month++)
            {
                // Calculate the start date of the month
                var monthStartDate = new DateTime(currentYear, month, 1).ToUniversalTime();

                // Calculate the end date of the month
                var monthEndDate = monthStartDate.AddMonths(1).AddDays(-1).ToUniversalTime();

                // Count users within the current month
                var userCount = await _messageLogRepo.Query(u => u.CreatedAt >= monthStartDate && u.CreatedAt <= monthEndDate).CountAsync();

                // Create a new MessageLogStatDto object for the current month and count
                var monthlyCountDto = new MessageLogStatByMonthDto
                {
                    Month = month,
                    Year = currentYear,
                    Count = userCount,
                    Channel = "WhatsApp"
                };

                // Add the monthly count to the list
                monthlyCounts.Add(monthlyCountDto);
            }

            return monthlyCounts;
        }


        private async Task<List<MessageLogStatByYearDto>> WhatsappMessageStatsForYears()
        {
            // Get the current year
            var currentYear = 2023;

            // Get the earliest record date from the database
            var earliestRecordDate = await _messageLogRepo.Query(u => u.CreatedAt != DateTime.MinValue).MinAsync(u => u.CreatedAt);
            var earliestRecordYear = earliestRecordDate.Year;

            // Initialize a list to store the yearly counts
            var yearlyCounts = new List<MessageLogStatByYearDto>();

            // Loop through each year from the current year to the earliest record year
            for (int year = currentYear; year >= earliestRecordYear; year--)
            {
                // Calculate the start date of the year
                var yearStartDate = new DateTime(year, 1, 1);

                // Calculate the end date of the year
                var yearEndDate = yearStartDate.AddYears(1).AddDays(-1);

                // Count users within the current year
                var userCount = await _messageLogRepo.Query(u => u.CreatedAt >= yearStartDate && u.CreatedAt <= yearEndDate).CountAsync();

                // Create a new MessageLogStatDto object for the current year and count
                var yearlyCountDto = new MessageLogStatByYearDto
                {
                    Year = year,
                    Count = userCount,
                    Channel = "WhatsApp"
                };

                // Add the yearly count to the list
                yearlyCounts.Add(yearlyCountDto);
            }

            return yearlyCounts;
        }

        #endregion

        //statistic count initializers
        #region Statistics Initializers

        private Task<List<T>> InitializeCountStatByWeek<T>() where T : new()
        {

            // Initialize the user counts for each day to 0
            var userCountPerDay = new List<T>();
            foreach (DayOfWeek dayOfWeek in Enum.GetValues(typeof(DayOfWeek)))
            {
                var entity = new T();
                typeof(T).GetProperty("DayOfWeek")?.SetValue(entity, dayOfWeek.ToString());
                typeof(T).GetProperty("Count")?.SetValue(entity, 0);
                typeof(T).GetProperty("Channel")?.SetValue(entity, "Whatsapp");
                userCountPerDay.Add(entity);
            }

            return Task.FromResult(userCountPerDay);
        }

        private Task<List<T>> InitializeCountStatByMonth<T>() where T : new()
        {

            // Initialize the user counts for each day to 0
            var userCountPerMonth = new List<T>();
            for (int month = 1; month <= 12; month++)
            {

                var entity = new T();
                typeof(T).GetProperty("Year")?.SetValue(entity, DateTime.UtcNow.Year);
                typeof(T).GetProperty("MonthOfYear")?.SetValue(entity, month);
                typeof(T).GetProperty("MonthName")?.SetValue(entity, CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month));
                typeof(T).GetProperty("Count")?.SetValue(entity, 0);
                userCountPerMonth.Add(entity);
            }
            return Task.FromResult(userCountPerMonth);
        }

        private Task<List<T>> InitializeCountStatByYear<T>() where T : new()
        {
            var currentYear = 2023;
            var startDate = new DateTime(currentYear, 1, 1).ToUniversalTime().AddHours(1); // First day of the current year
            // Initialize the user counts for each day to 0
            var userCountPerYear = new List<T>();
            for (int year = startDate.Year; year <= 2033; year++)
            {

                var entity = new T();
                typeof(T).GetProperty("Year")?.SetValue(entity, year);
                typeof(T).GetProperty("Count")?.SetValue(entity, 0);
                userCountPerYear.Add(entity);
            }
            return Task.FromResult(userCountPerYear);
        }

        #endregion

       



    }
}
