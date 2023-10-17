using System.Globalization;
using CsvHelper.Configuration;
using Flow.Infrastructure.IO.CSV.ExchangeRates;
using Flow.Infrastructure.IO.CSV.Transactions;
using Flow.Infrastructure.IO.CSV.Transactions.Transfers;
using FluentAssertions;
using Xunit.Categories;

namespace Flow.Infrastructure.IO.CSV.UnitTests;

public class CsvSerializersShould : TestDataCarrier
{
    [Theory, UnitTest]
    [InlineData("ru-RU")]
    [InlineData("en-US")]
    public async Task SerializeAndDeserializeTransactionsProperly(string cultureCode)
    {
        var culture = CultureInfo.GetCultureInfo(cultureCode);

        var config = new CsvConfiguration(culture);
        var rdr = new TransactionsReader(config);
        var wrtr = new TransactionsWriter(config);

        await using var stream = new MemoryStream();
        await using var writer = new StreamWriter(stream);

        await wrtr.Write(writer, Transactions, CancellationToken.None);
        stream.Seek(0, SeekOrigin.Begin);

        using var reader = new StreamReader(stream);

        var result = await rdr.Read(reader, CancellationToken.None);

        result.Select(r => r.Item1).Should().BeEquivalentTo(Transactions);
    }

    [Theory, UnitTest]
    [InlineData("ru-RU")]
    [InlineData("en-US")]
    public async Task SerializeAndDeserializeRecordedTransactionsProperly(string cultureCode)
    {
        var culture = CultureInfo.GetCultureInfo(cultureCode);
        var config = new CsvConfiguration(culture);
        var rdr = new RecordedTransactionsReader(config);
        var wrtr = new RecordedTransactionsWriter(config);

        await using var stream = new MemoryStream();
        await using var writer = new StreamWriter(stream);

        await wrtr.Write(writer, RecordedTransactions, CancellationToken.None);
        stream.Seek(0, SeekOrigin.Begin);

        using var reader = new StreamReader(stream);

        var result = await rdr.Read(reader, CancellationToken.None);

        result.ToList().Should().BeEquivalentTo(RecordedTransactions);
    }

    [Theory, UnitTest]
    [InlineData("ru-RU")]
    [InlineData("en-US")]
    public async Task SerializeAndDeserializeTransferKeysProperly(string cultureCode)
    {
        var culture = CultureInfo.GetCultureInfo(cultureCode);
        var config = new CsvConfiguration(culture);

        var rdr = new TransferKeyReader(config);
        var wrtr = new TransfersWriter(config);

        await using var stream = new MemoryStream();
        await using var writer = new StreamWriter(stream);

        await wrtr.Write(writer, TransferKeys, CancellationToken.None);
        stream.Seek(0, SeekOrigin.Begin);

        using var reader = new StreamReader(stream);

        var result = await rdr.Read(reader, CancellationToken.None);

        result.ToList().Should().BeEquivalentTo(TransferKeys);
    }

    [Theory, UnitTest]
    [InlineData("ru-RU")]
    [InlineData("en-US")]
    public async Task SerializeAndDeserializeExchangeRatesProperly(string cultureCode)
    {
        var culture = CultureInfo.GetCultureInfo(cultureCode);
        var config = new CsvConfiguration(culture);

        var rdr = new ExchangeRatesReader(config);
        var wrtr = new ExchangeRatesWriter(config);

        await using var stream = new MemoryStream();
        await using var writer = new StreamWriter(stream);

        await wrtr.Write(writer, Rates, CancellationToken.None);
        stream.Seek(0, SeekOrigin.Begin);

        using var reader = new StreamReader(stream);

        var result = await rdr.Read(reader, CancellationToken.None);

        result.ToList().Should().BeEquivalentTo(Rates);
    }
}
