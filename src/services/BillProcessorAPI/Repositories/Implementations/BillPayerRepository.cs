using BillProcessorAPI.Data;
using BillProcessorAPI.Entities;
using BillProcessorAPI.Repositories.Interfaces;

namespace BillProcessorAPI.Repositories.Implementations
{
	public class BillPayerRepository : Repository<BillPayerInfo> , IBillPayerRepository
	{
		public BillPayerRepository(BillProcessorDbContext context) : base(context)
		{
		}
	}
}
