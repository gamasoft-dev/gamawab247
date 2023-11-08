using System.Globalization;
using System.Net;
using Application.DTOs.PartnerContentDtos;
using Application.Helpers;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BillProcessorAPI.Dtos;
using BillProcessorAPI.Entities;
using BillProcessorAPI.Enums;
using BillProcessorAPI.Repositories.Interfaces;
using BillProcessorAPI.Services.Interfaces;
using Domain.Common;
using Infrastructure.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Math.EC;

namespace BillProcessorAPI.Services.Implementations
{
    public class CollectionReportService : ICollectionReportService
    {
        private readonly IBillTransactionRepository _billTransactionRepo;
        private readonly IMapper _mapper;
        private EGatewayType _paymentGateway;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly TransactionOptions _transactionOptions;

        public CollectionReportService(IBillTransactionRepository billTransactionRepo, IMapper mapper, IHttpContextAccessor contextAccessor, IOptions<TransactionOptions> transactionOptions)
        {
            _billTransactionRepo = billTransactionRepo;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _transactionOptions = transactionOptions.Value;
        }

        public async Task<PagedResponse<IEnumerable<CollectionReportDto>>> GetAllPagedCollections(ResourceParameter parameter, ReportParameters reportParam, string endPointName, IUrlHelper url)
        {

            var queryProjection = _billTransactionRepo.GetCollectionAllReport(parameter,reportParam);

            if (queryProjection == null)
                throw new RestException(System.Net.HttpStatusCode.BadRequest, ResponseMessages.Failed);

            var collections = await PagedList<CollectionReportDto>.CreateAsync(queryProjection, parameter.PageNumber, parameter.PageSize, parameter.Sort);
            var page = PageUtility<CollectionReportDto>.CreateResourcePageUrl(parameter, endPointName, collections, url);

            var response = new PagedResponse<IEnumerable<CollectionReportDto>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = collections,
                Meta = new Meta
                {
                    Pagination = page
                }
            };
            return response;

        }

        public async Task<TransactionDashboardStatsDto> GetAllCollections()
        {
            var request = _contextAccessor.HttpContext.Request;
            if (!request.Headers.ContainsKey(_transactionOptions.TransactionHeader) ||
                request.Headers[_transactionOptions.TransactionHeader] != _transactionOptions.TransactionHeaderValue)
            {
                throw new RestException(HttpStatusCode.Unauthorized, "Authorization failed");
            }

            // Calculate the start of the week (Sunday)
            var currentDate = DateTime.UtcNow.AddHours(1);
            int diff = currentDate.DayOfWeek - DayOfWeek.Sunday;
            if (diff < 0)
                diff += 7; // Handle negative difference when the current date is Sunday
            var currentWeekStartDate = currentDate.Date.AddDays(-diff);
            // Calculate the end of the week (Saturday)
            var currentWeekEndDate = currentWeekStartDate.AddDays(6);

            // Fetch all transactions from the database
            var allTransactions = await _billTransactionRepo.Query(u => u.CreatedAt != DateTime.MinValue && u.AmountPaid != 0).ToListAsync();

            // Get the earliest record date from all transactions
            var earliestTransactionDate = allTransactions.Min(t => t.CreatedAt);
            var currentMonth = new DateTime(earliestTransactionDate.Year, earliestTransactionDate.Month, 1);


            var result = new TransactionDashboardStatsDto
            {
                WeeklySummary = await GetWeeklyTransactionSummary(allTransactions, currentWeekStartDate, currentWeekEndDate),
                MonthlyTransactionSummary = await GetMonthlyTransactionStats(allTransactions),
                YearlyTransactionSummary = await GetYearlyTransactionStats(allTransactions),
            };

            return result;
        }

        private async Task<WeeklyTransactionSummaryDto> GetWeeklyTransactionSummary(List<BillTransaction> billTransactions, DateTime currentWeekStartDate, DateTime currntWeekendDate)
        {
            // Initialize a list to store the monthly statistics
            var monthlyStats = new List<TransactionStatisticsDto>();

            // Fetch transactions within the current week
            var transactionsInWeek = billTransactions.Where(t => t.CreatedAt >= currentWeekStartDate && t.CreatedAt <= currntWeekendDate).ToList()
                ?? throw new RestException(HttpStatusCode.BadRequest, "an error occuredc while processing your request");

            if (transactionsInWeek.Any())
            {
                // Calculate the low, average, and highest payment for the week
                var lowPayment = transactionsInWeek.Min(t => t.AmountPaid);
                var averagePayment = transactionsInWeek.Average(t => t.AmountPaid);
                var highestPayment = transactionsInWeek.Max(t => t.AmountPaid);
                var result = new WeeklyTransactionSummaryDto
                {
                    CurrentWeek = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.UtcNow.AddHours(1), CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday),
                    LowestPayment = lowPayment,
                    AveragePayment = Math.Round(averagePayment, 2),
                    HighestPayment = highestPayment
                };

                return await Task.FromResult(result);

            }

            return null;
        }

        private async Task<List<MonthlyTransactionSummaryDto>> GetMonthlyTransactionStats(List<BillTransaction> billTransactions)
        {
            if (!billTransactions.Any())
            {
                return null; // No valid transactions found
            }

            // Group transactions by year and month
            var transactionsByMonth = billTransactions
                .GroupBy(t => new { t.CreatedAt.Year, t.CreatedAt.Month })
                .OrderBy(group => group.Key.Year)
                .ThenBy(group => group.Key.Month)
                .ToList();

            var monthlyStats = new List<MonthlyTransactionSummaryDto>();

            foreach (var monthTransactions in transactionsByMonth)
            {
                var year = monthTransactions.Key.Year;
                var month = monthTransactions.Key.Month;

                // Calculate the low, average, and highest payment for the month
                var lowPayment = monthTransactions.Min(t => t.AmountPaid);
                var averagePayment = monthTransactions.Average(t => t.AmountPaid);
                var highestPayment = monthTransactions.Max(t => t.AmountPaid);

                // Create a new MonthlyTransactionSummaryDto object for the current month
                var monthlyStatsDto = new MonthlyTransactionSummaryDto
                {
                    Month = month,
                    Year = year,
                    LowestPayment = lowPayment,
                    AveragePayment = Math.Round(averagePayment, 2),
                    HighestPayment = highestPayment
                };

                // Add the monthly statistics to the list
                monthlyStats.Add(monthlyStatsDto);
            }

            return await Task.FromResult(monthlyStats);
        }
        
        private async Task<List<YearlyTransactionSummaryDto>> GetYearlyTransactionStats(List<BillTransaction> billTransactions)
        {
            if (!billTransactions.Any())
            {
                return null; // No valid transactions found
            }

            // Group transactions by year
            var transactionsByYear = billTransactions
                .GroupBy(t => t.CreatedAt.Year)
                .OrderBy(group => group.Key)
                .ToList();

            var currentYear = DateTime.Now.Year;
            var yearlyStats = new List<YearlyTransactionSummaryDto>();

            foreach (var yearTransactions in transactionsByYear)
            {
                var year = yearTransactions.Key;

                // Calculate the low, average, and highest payment for the year
                var lowPayment = yearTransactions.Min(t => t.AmountPaid);
                var averagePayment = yearTransactions.Average(t => t.AmountPaid);
                var highestPayment = yearTransactions.Max(t => t.AmountPaid);

                // Create a new YearlyTransactionSummaryDto object for the current year
                var yearlyStatsDto = new YearlyTransactionSummaryDto
                {
                    Year = year,
                    LowestPayment = lowPayment,
                    AveragePayment = Math.Round(averagePayment, 2),
                    HighestPayment = highestPayment
                };

                // Add the yearly statistics to the list
                yearlyStats.Add(yearlyStatsDto);
            }

            return await Task.FromResult(yearlyStats);
        }

    }
}

