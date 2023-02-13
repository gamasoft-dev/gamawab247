using BillProcessorAPI.Data;
using BillProcessorAPI.Entities;
using BillProcessorAPI.Repositories.Interfaces;

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
