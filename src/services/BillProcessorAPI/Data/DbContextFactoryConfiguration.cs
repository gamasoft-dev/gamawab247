using Microsoft.EntityFrameworkCore;

namespace BillProcessorAPI.Data
{
    public class DbContextFactoryConfiguration
    {
        public BillProcessorDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json")
                .AddJsonFile($"appsettings.Development.json")
                .SetBasePath(Directory.GetCurrentDirectory())
                .Build();

            var conn = config.GetConnectionString("DefaultConnection");

            var builder = new DbContextOptionsBuilder<BillProcessorDbContext>()
                .UseNpgsql(config.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("BillProcessorAPI"));

            return new BillProcessorDbContext(builder.Options);
        }
    }
}
