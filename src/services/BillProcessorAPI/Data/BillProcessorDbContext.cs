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

        public DbSet<User> Users { get; set; }
        public DbSet<BillTransaction> BillTransactions { get; set; }
    }
}
