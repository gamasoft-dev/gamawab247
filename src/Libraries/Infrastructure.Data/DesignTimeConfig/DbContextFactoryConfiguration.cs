using Infrastructure.Data.DbContext;
using Infrastructure.Data.DbContext.DbAuditFilters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.DesignTimeConfig
{
    internal class DbContextFactoryConfiguration : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json")
                .AddJsonFile($"appsettings.Development.json")
                .SetBasePath(Directory.GetCurrentDirectory())
                .Build();

            var conn = config.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseNpgsql(conn);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
