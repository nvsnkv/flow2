using System.Globalization;
using Autofac;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Calendar.Contract;
using Flow.Infrastructure.IO.Collections;
using Flow.Infrastructure.IO.Criteria.Contract;
using Flow.Infrastructure.IO.CSV.Contract;
using Flow.Infrastructure.IO.JSON.Contract;

namespace Flow.Infrastructure.IO.Contract;

public sealed class FlowIO : Module
{
    private readonly CultureInfo culture;
    private readonly DateTimeStyles dateStyle;
    private readonly NumberStyles numberStyle;

    public FlowIO(IFlowConfiguration config)
    {
        culture = CultureInfo
                      .GetCultures(CultureTypes.AllCultures)
                      .FirstOrDefault(ci => ci.Name == config.CultureCode)
                  ?? CultureInfo.CurrentCulture;

        numberStyle = Enum.TryParse(config.NumberStyle, out NumberStyles n) ? n : NumberStyles.Any;
        dateStyle = Enum.TryParse(config.DateStyle, out DateTimeStyles d) ? d : DateTimeStyles.AssumeLocal;
    }

    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterGeneric(typeof(Readers<>)).AsImplementedInterfaces().InstancePerLifetimeScope();
        builder.RegisterGeneric(typeof(Writers<>)).AsImplementedInterfaces().InstancePerLifetimeScope();

        builder.RegisterModule(new CSVIO(culture));
        builder.RegisterModule<JSONIO>();
        builder.RegisterModule(new CriteriaParsing(culture, dateStyle, numberStyle));
        builder.RegisterModule(new CalendarIO(culture));
    }
}
