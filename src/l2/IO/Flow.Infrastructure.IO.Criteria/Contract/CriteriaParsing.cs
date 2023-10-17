using System.Globalization;
using System.Runtime.CompilerServices;
using Autofac;
using Flow.Infrastructure.Configuration.Contract;

[assembly:InternalsVisibleTo("Flow.Infrastructure.IO.Criteria.UnitTests")]
namespace Flow.Infrastructure.IO.Criteria.Contract;

public class CriteriaParsing : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.Register(c =>
        {
            var config = c.Resolve<IFlowConfiguration>();

            var culture = CultureInfo
                              .GetCultures(CultureTypes.AllCultures)
                              .FirstOrDefault(ci => ci.Name == config.CultureCode)
                          ?? CultureInfo.CurrentCulture;

            var numberStyle = Enum.TryParse(config.NumberStyle, out NumberStyles n) ? n : NumberStyles.Any;
            var dateStyle = Enum.TryParse(config.DateStyle, out DateTimeStyles d) ? d : DateTimeStyles.AssumeLocal;

            return new TransactionCriteriaParser(new GenericParser(culture, dateStyle, numberStyle));
        }).InstancePerLifetimeScope().AsImplementedInterfaces();

        base.Load(builder);
    }
}
