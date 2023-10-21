using System.Globalization;
using System.Runtime.CompilerServices;
using Autofac;
using CsvHelper.Configuration;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Calendar;
using Flow.Infrastructure.IO.ExchangeRates;
using Flow.Infrastructure.IO.Generics;
using Flow.Infrastructure.IO.Transactions;
using Flow.Infrastructure.IO.Transactions.Contract;
using Flow.Infrastructure.IO.Transactions.Criteria;
using Flow.Infrastructure.IO.Transactions.Transfers;
using Newtonsoft.Json;
using JsonSerializer = Flow.Infrastructure.IO.Generics.JsonSerializer;

[assembly: InternalsVisibleTo("Flow.Infrastructure.IO.UnitTests")]

namespace Flow.Infrastructure.IO.Contract;

public class FlowIOComponents : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        #region generics
        builder.Register(c =>
        {
            var config = c.Resolve<IFlowConfiguration>();
            var culture = CultureInfo
                              .GetCultures(CultureTypes.AllCultures)
                              .FirstOrDefault(ci => ci.Name == config.CultureCode)
                          ?? CultureInfo.CurrentCulture;

            return new CsvConfiguration(culture) { HeaderValidated = null };
        });

        builder.RegisterType<CsvSerializer>().InstancePerLifetimeScope();

        builder.Register(c =>
        {
            var config = c.Resolve<IFlowConfiguration>();
            var culture = CultureInfo
                              .GetCultures(CultureTypes.AllCultures)
                              .FirstOrDefault(ci => ci.Name == config.CultureCode)
                          ?? CultureInfo.CurrentCulture;

            return new JsonSerializerSettings { Culture = culture, Formatting = Formatting.Indented };
        });
        builder.RegisterType<JsonSerializer>().InstancePerLifetimeScope();
        #endregion

        #region transactions
        builder.RegisterType<DefaultJsonTransactionsSerializer>().InstancePerLifetimeScope().AsImplementedInterfaces();

        builder.RegisterType<CsvRejectionsWriter>().InstancePerLifetimeScope();
        builder.RegisterType<JsonRejectionsWriter>().InstancePerLifetimeScope();
        builder.RegisterType<RejectionsWriter>().InstancePerLifetimeScope().AsImplementedInterfaces();

        builder.RegisterType<TransfersWriter>().InstancePerLifetimeScope();
        builder.RegisterType<DefaultCsvTransactionsSerializer>().InstancePerLifetimeScope().AsImplementedInterfaces();
        builder.RegisterType<DefaultJsonTransactionsSerializer>().InstancePerLifetimeScope().AsImplementedInterfaces();

        builder.RegisterType<SchemaSpecificCollection<ITransactionsReader>>().InstancePerLifetimeScope().AsImplementedInterfaces();
        builder.RegisterType<SchemaSpecificCollection<ITransactionsWriter>>().InstancePerLifetimeScope().AsImplementedInterfaces();

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
        #endregion

        #region calendar

        #endregion

        #region exchange rates
        builder.RegisterType<ExchangeRatesSerializer>().InstancePerLifetimeScope().AsImplementedInterfaces();
        builder.RegisterType<RateRejectionsWriter>().InstancePerLifetimeScope().AsImplementedInterfaces();
        #endregion

        base.Load(builder);
    }
}
