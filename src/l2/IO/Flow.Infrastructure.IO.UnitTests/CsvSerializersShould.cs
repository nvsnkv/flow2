using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using Flow.Domain.ExchangeRates;
using Flow.Domain.Transactions;
using Flow.Domain.Transactions.Transfers;
using Flow.Infrastructure.IO.ExchangeRates;
using Flow.Infrastructure.IO.Generics;
using Flow.Infrastructure.IO.Transactions;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

namespace Flow.Infrastructure.IO.UnitTests;

public class CsvSerializersShould : TestDataCarrier
{
    [Theory, UnitTest]
    [InlineData("ru-RU")]
    [InlineData("en-US")]
    public async Task SerializeAndDeserializeTransactionsProperly(string cultureCode)
    {
        var culture = CultureInfo.GetCultureInfo(cultureCode);
        var serializer = new CsvSerializer(new CsvConfiguration(culture));

        await using var stream = new MemoryStream();
        await using var writer = new StreamWriter(stream);

        await serializer.Write<Transaction, TransactionRow, TransactionRowMap>(writer, Transactions, t => (TransactionRow)t, CancellationToken.None);
        stream.Seek(0, SeekOrigin.Begin);

        using var reader = new StreamReader(stream);

        var result = await serializer.Read(reader, (TransactionRow r) => (Transaction)r, CancellationToken.None);

        result.ToList().Should().BeEquivalentTo(Transactions);
    }

    [Theory, UnitTest]
    [InlineData("ru-RU")]
    [InlineData("en-US")]
    public async Task SerializeAndDeserializeRecordedTransactionsProperly(string cultureCode)
    {
        var culture = CultureInfo.GetCultureInfo(cultureCode);
        var serializer = new CsvSerializer(new CsvConfiguration(culture));

        await using var stream = new MemoryStream();
        await using var writer = new StreamWriter(stream);

        await serializer.Write<RecordedTransaction, RecordedTransactionRow, RecordedTransactionRowMap>(writer, RecordedTransactions, t => (RecordedTransactionRow)t, CancellationToken.None);
        stream.Seek(0, SeekOrigin.Begin);

        using var reader = new StreamReader(stream);

        var result = await serializer.Read(reader, (RecordedTransactionRow r) => (RecordedTransaction)r, CancellationToken.None);

        result.ToList().Should().BeEquivalentTo(RecordedTransactions);
    }

    [Theory, UnitTest]
    [InlineData("ru-RU")]
    [InlineData("en-US")]
    public async Task SerializeAndDeserializeTransferKeysProperly(string cultureCode)
    {
        var culture = CultureInfo.GetCultureInfo(cultureCode);
        var serializer = new CsvSerializer(new CsvConfiguration(culture));

        await using var stream = new MemoryStream();
        await using var writer = new StreamWriter(stream);

        await serializer.Write(writer, TransferKeys, t => (TransferKeyRow)t, CancellationToken.None);
        stream.Seek(0, SeekOrigin.Begin);

        using var reader = new StreamReader(stream);

        var result = await serializer.Read<TransferKey, TransferKeyRow>(reader, r => (TransferKey)r, CancellationToken.None);

        result.ToList().Should().BeEquivalentTo(TransferKeys);
    }

    [Theory, UnitTest]
    [InlineData("ru-RU")]
    [InlineData("en-US")]
    public async Task SerializeAndDeserializeExchangeRatesProperly(string cultureCode)
    {
        var culture = CultureInfo.GetCultureInfo(cultureCode);
        var serializer = new CsvSerializer(new CsvConfiguration(culture));

        await using var stream = new MemoryStream();
        await using var writer = new StreamWriter(stream);

        await serializer.Write(writer, Rates, r => (ExchangeRateRow)r, CancellationToken.None);
        stream.Seek(0, SeekOrigin.Begin);

        using var reader = new StreamReader(stream);

        var result = await serializer.Read(reader, (ExchangeRateRow r) => (ExchangeRate)r, CancellationToken.None);

        result.ToList().Should().BeEquivalentTo(Rates);
    }
}