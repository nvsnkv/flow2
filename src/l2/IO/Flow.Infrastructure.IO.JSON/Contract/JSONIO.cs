using System.Globalization;
using System.Runtime.CompilerServices;
using Autofac;
using Flow.Infrastructure.IO.Contract;
using Flow.Infrastructure.IO.JSON.ExchangeRates;
using Flow.Infrastructure.IO.JSON.Transactions;
using Flow.Infrastructure.IO.JSON.Transactions.Transfers;
using Newtonsoft.Json;

[assembly: InternalsVisibleTo("Flow.Infrastructure.IO.JSON.UnitTests")]
namespace Flow.Infrastructure.IO.JSON.Contract;

public class JSONIO : Module
{
    private readonly CultureInfo culture;

    public static readonly SupportedFormat SupportedFormat = new("JSON");

    public JSONIO(CultureInfo culture)
    {
        this.culture = culture;
    }

    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterInstance(new JsonSerializerSettings { Culture = culture, Formatting = Formatting.Indented });

        builder.RegisterType<TransactionsReader>().AsImplementedInterfaces();
        builder.RegisterType<RecordedTransactionsReader>().AsImplementedInterfaces();

        builder.RegisterType<TransferKeyReader>().AsImplementedInterfaces();

        builder.RegisterType<ExchangeRatesReader>().AsImplementedInterfaces();

        builder.RegisterGeneric(typeof(JsonWriter<>)).As(typeof(IFormatSpecificWriter<>));

        base.Load(builder);
    }
}
