using System.IO;
using Infrastructure.Data.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace API.ContextFactory;


public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.Development.json", reloadOnChange: true, optional: true)
            .AddJsonFile($"appsettings.Production.json", reloadOnChange: true, optional: true)
            .AddEnvironmentVariables()
            .Build();

        var builder = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(config.GetConnectionString("DefaultConnection"), 
                b => b.MigrationsAssembly("Infrastructure.Data"));
        return new AppDbContext(builder.Options);
    }
}