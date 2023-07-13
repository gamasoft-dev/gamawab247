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
          
            var queryProjection = _billTransactionRepo.GetCollectionAllReport(reportParam);
       
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
