using System.Globalization;
using System.Runtime.CompilerServices;
using Autofac;
using CsvHelper.Configuration;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Criteria;
using Newtonsoft.Json;

[assembly:InternalsVisibleTo("Flow.Infrastructure.IO.UnitTests")]

namespace Flow.Infrastructure.IO;

public class IOComponents : Module
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

            var csv = new CsvTransactionsSerializer(new CsvConfiguration(culture));
            var json = new JsonTransactionsSerializer(new JsonSerializerSettings { Culture = culture });
            return new TransactionsIOFacade(csv, json);
        }).InstancePerLifetimeScope().AsImplementedInterfaces();

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