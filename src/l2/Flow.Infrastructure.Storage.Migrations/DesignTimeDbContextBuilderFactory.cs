using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Flow.Infrastructure.Storage.Migrations;

internal class DesignTimeDbContextBuilderFactory : IDesignTimeDbContextFactory<FlowDbContext>
{
    public FlowDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder()
            .UseNpgsql(
                args.FirstOrDefault() ?? throw new InvalidOperationException("Connection string was not configured!"), 
                o => o.MigrationsAssembly(typeof(DesignTimeDbContextBuilderFactory).Assembly.FullName))
            .Options;

        return new FlowDbContext(options);
    }
} 