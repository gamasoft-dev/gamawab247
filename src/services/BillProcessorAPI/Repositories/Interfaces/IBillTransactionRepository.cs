using System.Linq.Expressions;
using BillProcessorAPI.Entities;
using static Amazon.S3.Util.S3EventNotification;

namespace BillProcessorAPI.Repositories.Interfaces
{
    public interface IBillTransactionRepository : IRepository<BillTransaction>
    {
    }
}
