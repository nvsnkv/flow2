using System.Runtime.CompilerServices;
using Autofac;

[assembly: InternalsVisibleTo("Flow.Application.Analysis.UnitTests")]

namespace Flow.Application.Analysis.Contract;

public class Analysis : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<Analyst>().AsImplementedInterfaces();
        base.Load(builder);
    }
}