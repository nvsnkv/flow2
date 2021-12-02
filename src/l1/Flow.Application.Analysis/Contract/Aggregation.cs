using System.Runtime.CompilerServices;
using Autofac;

[assembly:InternalsVisibleTo("Flow.Application.Analysis.UnitTests")]

namespace Flow.Application.Analysis.Contract;

public class Aggregation : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<Aggregator>().AsImplementedInterfaces();

        base.Load(builder);
    }
}