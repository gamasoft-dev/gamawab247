using System.Linq;
using System.Linq.Expressions;
using BillProcessorAPI.Data;
using BillProcessorAPI.Entities;
using BillProcessorAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using static Amazon.S3.Util.S3EventNotification;

namespace BillProcessorAPI.Repositories.Implementations
{
    public class BillTransactionRespository : Repository<BillTransaction>, IBillTransactionRepository
    {
        public BillTransactionRespository(BillProcessorDbContext context)
            : base(context)
        {

        }
    }
}
