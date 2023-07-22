using System.Linq.Expressions;
using Application.AutofacDI;
using Application.Helpers;
using BillProcessorAPI.Dtos;
using BillProcessorAPI.Entities;
using Domain.Entities;
using static Amazon.S3.Util.S3EventNotification;

namespace BillProcessorAPI.Repositories.Interfaces
{
    public interface IBillTransactionRepository : IRepository<BillTransaction>
    {
        IQueryable<CollectionReportDto> GetCollectionAllReport(ResourceParameter parameter, ReportParameters reportParameter);
    }
}
