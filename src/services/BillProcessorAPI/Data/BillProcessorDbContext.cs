using BillProcessorAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace BillProcessorAPI.Data
{
    public class BillProcessorDbContext : DbContext
    {
        public BillProcessorDbContext(DbContextOptions options) 
            : base(options)
        {
        }

        public DbSet<BillPayerInfo> BillPayers { get; set; }
        public DbSet<BillTransaction> BillTransactions { get; set; }
        public DbSet<BillCharge> Charges { get; set; }
    }
}
