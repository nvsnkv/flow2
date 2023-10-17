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

    protected override void Load(ContainerBuilder builder)
    {
        builder.Register(c =>
        {
            var config = c.Resolve<IFlowConfiguration>();
            var culture = CultureInfo
                              .GetCultures(CultureTypes.AllCultures)
                              .FirstOrDefault(ci => ci.Name == config.CultureCode)
                          ?? CultureInfo.CurrentCulture;

            return new CsvConfiguration(culture) { HeaderValidated = null };
        }).SingleInstance();

        builder.RegisterAssemblyTypes().AsImplementedInterfaces();

        base.Load(builder);
    }
}
