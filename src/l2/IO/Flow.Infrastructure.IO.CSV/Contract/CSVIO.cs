using System.Globalization;
using System.Runtime.CompilerServices;
using Autofac;
using CsvHelper.Configuration;
using Flow.Domain.Transactions;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Contract;
using Flow.Infrastructure.IO.CSV.ExchangeRates;
using Flow.Infrastructure.IO.CSV.Transactions;
using Flow.Infrastructure.IO.CSV.Transactions.Transfers;

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

        builder.RegisterType<TransactionsReader>().As<IFormatSpecificReader<(Transaction, Overrides?)>>();
        builder.RegisterType<TransactionsWriter>().AsImplementedInterfaces();
        builder.RegisterType<RejectedTransactionsWriter>().AsImplementedInterfaces();

        builder.RegisterType<RecordedTransactionsReader>().AsImplementedInterfaces();
        builder.RegisterType<RecordedTransactionsWriter>().AsImplementedInterfaces();

        builder.RegisterType<TransferKeyReader>().AsImplementedInterfaces();
        builder.RegisterType<TransfersWriter>().AsImplementedInterfaces();
        builder.RegisterType<RejectedTransfersWriter>().AsImplementedInterfaces();

        builder.RegisterType<ExchangeRatesReader>().AsImplementedInterfaces();
        builder.RegisterType<ExchangeRatesWriter>().AsImplementedInterfaces();
        builder.RegisterType<RejectedRatesWriter>().AsImplementedInterfaces();

        base.Load(builder);
    }
}
