using System.Globalization;
using System.Runtime.CompilerServices;
using Autofac;
using Flow.Infrastructure.Configuration.Contract;

[assembly:InternalsVisibleTo("Flow.Infrastructure.IO.Criteria.UnitTests")]
namespace Flow.Infrastructure.IO.Criteria.Contract;

public class CriteriaParsing : Module
{
    private readonly CultureInfo culture;
    private readonly DateTimeStyles dateStyle;
    private readonly NumberStyles numberStyle;

    public CriteriaParsing(CultureInfo culture, DateTimeStyles dateStyle, NumberStyles numberStyle)
    {
        this.culture = culture;
        this.dateStyle = dateStyle;
        this.numberStyle = numberStyle;
    }

    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterInstance(new TransactionCriteriaParser(new GenericParser(culture, dateStyle, numberStyle))).AsImplementedInterfaces();

        base.Load(builder);
    }
}
