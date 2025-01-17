﻿using Application.DTOs.PartnerContentDtos;
using Application.Helpers;
using BillProcessorAPI.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace BillProcessorAPI.Services.Interfaces
{
    public interface ICollectionReportService
    {
        Task<PagedResponse<IEnumerable<CollectionReportDto>>> GetAllPagedCollections(ResourceParameter parameter, ReportParameters reportParam,
            string endPointName, IUrlHelper url);
        Task<TransactionDashboardStatsDto> GetAllCollections();

    }
}
