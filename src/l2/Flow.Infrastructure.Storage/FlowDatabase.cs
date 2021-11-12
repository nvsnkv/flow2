using System.Runtime.CompilerServices;
using Autofac;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.Storage.Model;
using Microsoft.EntityFrameworkCore;

[assembly:InternalsVisibleTo("Flow.Infrastructure.Storage.Migrations")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Flow.Infrastructure.Storage;

public class FlowDatabase : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.Register(c =>
        {
            var config = c.Resolve<IFlowConfiguration>();
            return new FlowDbContextFactory(config);
        }).InstancePerLifetimeScope().AsImplementedInterfaces();

        builder.Register(c => new TransactionStorage(c.Resolve<IDbContextFactory<FlowDbContext>>())).InstancePerLifetimeScope().AsImplementedInterfaces();
        builder.Register(c => new TransferOverridesStorage(c.Resolve<IDbContextFactory<FlowDbContext>>())).InstancePerLifetimeScope().AsImplementedInterfaces();
         
        base.Load(builder);
    }
}