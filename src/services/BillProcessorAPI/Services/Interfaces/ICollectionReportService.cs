using Application.DTOs.PartnerContentDtos;
using BillProcessorAPI.Dtos;
using BillProcessorAPI.Entities.ValueObjects;
using BillProcessorAPI.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace BillProcessorAPI.Services.Interfaces
{
    public interface ICollectionReportService
    {
        Task<PagedResponse<IEnumerable<CollectionReportDto>>> GetAllCollections(ResourceParameter parameter, ReportParameters reportParam,
            string endPointName, IUrlHelper url);

    }
}
