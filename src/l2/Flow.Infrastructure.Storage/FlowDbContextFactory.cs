using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.Storage.Model;
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
            .Options;

        return new FlowDbContext(options);
    }
}