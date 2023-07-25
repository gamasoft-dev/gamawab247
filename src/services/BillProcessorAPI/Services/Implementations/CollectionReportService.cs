using System.Globalization;
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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BillProcessorAPI.Services.Implementations
{
    public class CollectionReportService : ICollectionReportService
    {
        private readonly IBillTransactionRepository _billTransactionRepo;
        private readonly IMapper _mapper;
        private EGatewayType _paymentGateway;
        public CollectionReportService(IBillTransactionRepository billTransactionRepo, IMapper mapper)
        {
            _billTransactionRepo = billTransactionRepo;
            _mapper = mapper;
        }

        public async Task<PagedResponse<IEnumerable<CollectionReportDto>>> GetAllCollections(ResourceParameter parameter, ReportParameters reportParam, string endPointName, IUrlHelper url)
        {

            //int selectedStartYear = parameter.StartDate?.Year ?? 0;
            //int selectedStartMonth = parameter.StartDate?.Month ?? 0;
            //int selectedEndYear = parameter.EndDate?.Year ?? 0;
            //int month = 1;
            //var filterStartDate = new DateTime(selectedStartYear, selectedStartMonth, 01).ToUniversalTime();
            //var filterEndDate = new DateTime(selectedEndYear, month, DateTime.DaysInMonth(selectedEndYear, month), 23, 59, 59).ToUniversalTime();
           
       
           
            
            var queryProjection = _billTransactionRepo.GetCollectionAllReport(parameter, reportParam);
       
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
    }
}
