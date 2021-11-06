﻿using Autofac;
using Flow.Infrastructure.Configuration.Contract;
using Microsoft.EntityFrameworkCore;

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
         
        base.Load(builder);
    }
}