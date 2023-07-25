using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.AutofacDI;
using Application.DTOs.DashboardDtos;
using Application.Helpers;
using Domain.Common;

namespace Application.Services.Interfaces.Dashboard
{
    public interface IDashboardStatisticsService : IAutoDependencyService
    {
        Task<SuccessResponse<GetStatisticsDto>> GetStatistics(DateTime? startDate, DateTime? endDate);
    }
}
