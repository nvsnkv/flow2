using System.Globalization;
using Autofac;
using Flow.Application.Transactions.Contract;
using Flow.Domain.ExchangeRates;
using Flow.Domain.Transactions;
using Flow.Domain.Transactions.Transfers;
using Flow.Infrastructure.IO.Contract;
using Flow.Infrastructure.IO.CSV.Contract;
using FluentAssertions;

namespace Flow.Infrastructure.IO.CSV.UnitTests;

public class CSVIOShould
{
    private readonly IContainer container;

    public CSVIOShould()
    {
        var builder = new ContainerBuilder();
        builder.RegisterModule(new CSVIO(CultureInfo.CurrentCulture));

        container = builder.Build();
    }

    [Fact] public void RegisterIncomingTransactionsReader() => AssertRegistrationValid<IFormatSpecificReader<IncomingTransaction>, IncomingTransaction>();
    [Fact] public void RegisterRecordedTransactionsReader() => AssertRegistrationValid<IFormatSpecificReader<RecordedTransaction>, RecordedTransaction>();
    [Fact] public void RegisterTransferKeysReader() => AssertRegistrationValid<IFormatSpecificReader<TransferKey>, TransferKey>();
    [Fact] public void RegisterExchangeRatesReader() => AssertRegistrationValid<IFormatSpecificReader<ExchangeRate>, ExchangeRate>();

    [Fact] public void RegisterTransactionsWriter() => AssertRegistrationValid<IFormatSpecificWriter<Transaction>, Transaction>();
    [Fact] public void RegisterRejectedTransactionWriter() => AssertRegistrationValid<IFormatSpecificWriter<RejectedTransaction>, RejectedTransaction>();
    [Fact] public void RegisterRecordedTransactionWriter() => AssertRegistrationValid<IFormatSpecificWriter<RecordedTransaction>, RecordedTransaction>();
    [Fact] public void RegisterTransfersWriter() => AssertRegistrationValid<IFormatSpecificWriter<TransferKey>, TransferKey>();
    [Fact] public void RegisterRejectedTransfersWriter() => AssertRegistrationValid<IFormatSpecificWriter<RejectedTransferKey>, RejectedTransferKey>();
    [Fact] public void RegisterExchangeRatesWriter() => AssertRegistrationValid<IFormatSpecificWriter<ExchangeRate>, ExchangeRate>();
    [Fact] public void RegisterRejectedRatesWriter() => AssertRegistrationValid<IFormatSpecificWriter<RejectedRate>, RejectedRate>();

    private void AssertRegistrationValid<T,TE>() where T : class, IFormatSpecific
    {
        var readers = container.Resolve<IEnumerable<T>>().ToList();
        readers.Should().HaveCount(1);
        var reader = readers.First();
        reader.Should().NotBeNull();
        reader.Format.Should().Be(CSVIO.SupportedFormat);
    }
}
