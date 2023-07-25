using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3.Model;
using Application.DTOs.DashboardDtos;
using Application.Helpers;
using Application.Services.Interfaces.Dashboard;
using Domain.Entities;
using Domain.Entities.Identities;
using Domain.Entities.RequestAndComplaints;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementations.Dashboard
{
    public class DashboardStatisticsService : IDashboardStatisticsService
    {
        private readonly IRepository<RequestAndComplaint> _requestAndComplaintRepo;
        private readonly IWhatsappUserRepository _whatsappUserRepo;

        public DashboardStatisticsService(IRepository<RequestAndComplaint> requestAndComplaintRepo, IWhatsappUserRepository whatsappUserRepo)
        {
            _requestAndComplaintRepo = requestAndComplaintRepo;
            _whatsappUserRepo = whatsappUserRepo;
        }

        //Get Request and complaint statistics
        public async Task<SuccessResponse<GetStatisticsDto>> GetStatistics(DateTime? startDate, DateTime? endDate)
        {
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

            stats.RequestAndComplaintDataEnterPieChart.Add(new ChartSeries { Name = "Request", ResolutionChartData = requestChartData });
            stats.RequestAndComplaintDataEnterPieChart.Add(new ChartSeries { Name = "Complaint", ResolutionChartData = complaintChartData });
            stats.CustomerCountsPerDay = await GetWhatsappCustomerCount(startDate, endDate);

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

        private async Task<List<WhatsappCustomerStatsDto>> GetWhatsappCustomerCount(DateTime? startDate, DateTime? endDate)
        {
            var startDateUtc = startDate?.ToUniversalTime();
            var endDateUtc = endDate?.ToUniversalTime();
            if(startDate > endDate)
            {
                throw new RestException(HttpStatusCode.BadRequest,"Invalid date range. The start date must be before or equal to the end date.");
            }
            // Group users by the day of the week and get the counts for each day
            var userCountsPerWeek = await _whatsappUserRepo.Query(u => u.CreatedAt >= startDateUtc && u.CreatedAt <= endDateUtc).ToListAsync();

            // Initialize the user counts for each day to 0
            var userCountPerDay = new List<WhatsappCustomerStatsDto>();
            foreach (DayOfWeek dayOfWeek in Enum.GetValues(typeof(DayOfWeek)))
            {
                userCountPerDay.Add(new WhatsappCustomerStatsDto
                {
                    DayOfWeek = dayOfWeek.ToString(),
                    Count = 0
                });
            }

            // Add users to the list by days
            foreach (var user in userCountsPerWeek)
            {
                var dayOfWeek = user.CreatedAt.DayOfWeek.ToString();
                var userCountDto = userCountPerDay.Find(dto => dto.DayOfWeek == dayOfWeek);
                if (userCountDto != null)
                {
                    userCountDto.Count++;
                }
            }

            return userCountPerDay;
        }
    }
}
