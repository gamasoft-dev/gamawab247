using System.Linq;
using System.Linq.Expressions;
using Application.Helpers;
using BillProcessorAPI.Data;
using BillProcessorAPI.Dtos;
using BillProcessorAPI.Entities;
using BillProcessorAPI.Enums;
using BillProcessorAPI.Repositories.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using static Amazon.S3.Util.S3EventNotification;

namespace BillProcessorAPI.Repositories.Implementations
{
    public class BillTransactionRespository : Repository<BillTransaction>, IBillTransactionRepository
    {
        private EGatewayType _paymentGateway;
        public BillTransactionRespository(BillProcessorDbContext context)
            : base(context)
        {

        }

        public IQueryable<CollectionReportDto> GetCollectionAllReport(ResourceParameter parameter,ReportParameters reportParameter)
        {

            var query = _context.BillTransactions.OrderByDescending(x=>x.UpdatedAt).IgnoreQueryFilters() as IQueryable<BillTransaction>;

            if (!string.IsNullOrEmpty(reportParameter.SearchText))
            {
                query = query.Where(x => x.PayerName.ToLower().Contains(reportParameter.SearchText.ToLower()));
            }

            if (!string.IsNullOrEmpty(reportParameter.PaymentGateway))
            {
                bool convertPaymentGateway = Enum.TryParse(value: reportParameter.PaymentGateway, ignoreCase: true, out _paymentGateway);
                query = query.Where(x => x.GatewayType == _paymentGateway).IgnoreQueryFilters() as IQueryable<BillTransaction>;

            }

            if (parameter.StartDate != null && parameter.EndDate != null)
            {
                var filterStartDate = parameter.StartDate?.ToUniversalTime().AddHours(1);
                var filterEndDate = parameter.EndDate?.ToUniversalTime().AddDays(1).AddMinutes(59);
                query = query.Where(x => x.CreatedAt >= filterStartDate && x.CreatedAt <= filterEndDate).IgnoreQueryFilters() as IQueryable<BillTransaction>;
            }

           
            var collection = query.Select(n => (CollectionReportDto)n);
            return collection;
        }

      
    }
}
