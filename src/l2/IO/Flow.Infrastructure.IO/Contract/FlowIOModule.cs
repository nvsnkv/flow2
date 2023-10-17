using Autofac;
using Flow.Infrastructure.IO.Calendar.Contract;
using Flow.Infrastructure.IO.Collections;
using Flow.Infrastructure.IO.Criteria.Contract;
using Flow.Infrastructure.IO.CSV.Contract;
using Flow.Infrastructure.IO.JSON.Contract;

namespace Flow.Infrastructure.IO.Contract;

public sealed class FlowIOModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterGeneric(typeof(Readers<>)).AsImplementedInterfaces().InstancePerLifetimeScope();
        builder.RegisterGeneric(typeof(Writers<>)).AsImplementedInterfaces().InstancePerLifetimeScope();

        builder.RegisterModule<CSVIO>();
        builder.RegisterModule<JSONIO>();
        builder.RegisterModule<CriteriaParsing>();
        builder.RegisterModule<CalendarIO>();
    }
}
