using Flow.Infrastructure.Configuration;
using Flow.Infrastructure.Configuration.Contract;
using Microsoft.EntityFrameworkCore;

namespace Flow.Infrastructure.Storage;

internal class FlowDbContextFactory : IDbContextFactory<FlowDbContext>
{
    private readonly IFlowConfiguration config;

    public FlowDbContextFactory(IFlowConfiguration config)
    {
        this.config = config;
    }

    public FlowDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder()
            .UseNpgsql(config.ConnectionString ?? throw new InvalidOperationException("Connection string was not configured!"))
            .UseLazyLoadingProxies()
            .Options;

        return new FlowDbContext(options);
    }
}