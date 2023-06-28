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

        public IQueryable<CollectionReportDto> GetCollectionAllReport(ReportParameters reportParameter)
        {
            var query = _context.BillTransactions.OrderByDescending(x=>x.UpdatedAt).IgnoreQueryFilters() as IQueryable<BillTransaction>;
            var collection = query.Select(n => (CollectionReportDto)n);
            if (!string.IsNullOrEmpty(reportParameter.PaymentGateway))
            {
                bool convertPaymentGateway = Enum.TryParse<EGatewayType>(value: reportParameter.PaymentGateway, ignoreCase: true, out _paymentGateway);
                collection.Where(x => x.GatewayType == (int)_paymentGateway);

            }


            return collection;
        }
    }
}
