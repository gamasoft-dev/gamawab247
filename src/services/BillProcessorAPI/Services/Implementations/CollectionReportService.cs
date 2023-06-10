using System.Globalization;
using Application.DTOs.PartnerContentDtos;
using Application.Helpers;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BillProcessorAPI.Dtos;
using BillProcessorAPI.Entities;
using BillProcessorAPI.Repositories.Interfaces;
using BillProcessorAPI.Services.Interfaces;
using Domain.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BillProcessorAPI.Services.Implementations
{
    public class CollectionReportService : ICollectionReportService
    {
        private readonly IRepository<BillTransaction> _billTransactionRepo;
        private readonly IMapper _mapper;
        public CollectionReportService(IRepository<BillTransaction> billTransactionRepo, IMapper mapper)
        {
            _billTransactionRepo = billTransactionRepo;
            _mapper = mapper;
        }

        public async Task<PagedResponse<IEnumerable<CollectionReportDto>>> GetAllCollections(ResourceParameter parameter, ReportParameters reportParam, string endPointName, IUrlHelper url)
        {
            var queryable = _billTransactionRepo
               .Query(x => string.IsNullOrEmpty(parameter.Search)
                           || x.PayerName.ToLower().Contains(parameter.Search.ToLower()) 
                           || DateTime.Parse(x.DateCompleted) >= parameter.StartDate 
                           || DateTime.Parse(x.DateCompleted) <= parameter.EndDate);
            //DateOnly result;
            //string format = "yyyy-MM-dd";
            //CultureInfo culture = CultureInfo.InvariantCulture;
            //DateTimeStyles styles = DateTimeStyles.None;

            //queryable.Where(x => DateOnly.TryParse(x.DateCompleted.ToString(), out result));

            if (parameter.StartDate != DateTime.MinValue)
            {
                queryable.Where(x => x.UpdatedAt >= parameter.StartDate && x.UpdatedAt <= parameter.EndDate);
            };
            
            if (queryable == null)
                throw new RestException(System.Net.HttpStatusCode.BadRequest, ResponseMessages.Failed);

            var queryProjection = queryable.OrderByDescending(x => x.UpdatedAt).Select(x => (CollectionReportDto)x);

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
