using System.Globalization;
using System.Runtime.CompilerServices;
using Autofac;
using CsvHelper.Configuration;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Contract;

[assembly:InternalsVisibleTo("Flow.Infrastructure.IO.CSV.UnitTests")]
namespace Flow.Infrastructure.IO.CSV.Contract;

public sealed class CSVIO : Module
{
    public static readonly SupportedFormat SupportedFormat = new("CSV");

    private readonly CultureInfo culture;

    public CSVIO(CultureInfo culture)
    {
        this.culture = culture;
    }

    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterInstance(new CsvConfiguration(culture) { HeaderValidated = null });
        builder.RegisterAssemblyTypes().AsImplementedInterfaces();

        base.Load(builder);
    }
}
