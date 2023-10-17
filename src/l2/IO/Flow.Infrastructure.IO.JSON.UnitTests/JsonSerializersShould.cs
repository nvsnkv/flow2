using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flow.Domain.ExchangeRates;
using Flow.Domain.Transactions;
using Flow.Domain.Transactions.Transfers;
using Flow.Infrastructure.IO.JSON.ExchangeRates;
using Flow.Infrastructure.IO.JSON.Transactions;
using Flow.Infrastructure.IO.JSON.Transactions.Transfers;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;
using Xunit.Categories;

namespace Flow.Infrastructure.IO.JSON.UnitTests;

public class JsonSerializersShould : TestDataCarrier
{
    [Theory, UnitTest]
    [InlineData("ru-RU")]
    [InlineData("en-US")]
    public async Task SerializeAndDeserializeTransactionsProperly(string cultureCode)
    {
        var culture = CultureInfo.GetCultureInfo(cultureCode);
        var settings = new JsonSerializerSettings() { Culture = culture };

        var rdr = new TransactionsReader(settings);
        var wrtr = new JsonWriter<Transaction>(settings);

        await using var writeStream = new MemoryStream();
        await using var writer = new StreamWriter(writeStream);

        await wrtr.Write(writer, Transactions, CancellationToken.None);
        await using var readStream = new MemoryStream(writeStream.ToArray());
        
        using var reader = new StreamReader(readStream);

        var result = await rdr.Read(reader, CancellationToken.None);

        result.ToList().Should().BeEquivalentTo(Transactions);
    }

    [Theory, UnitTest]
    [InlineData("ru-RU")]
    [InlineData("en-US")]
    public async Task SerializeAndDeserializeRecordedTransactionsProperly(string cultureCode)
    {
        var culture = CultureInfo.GetCultureInfo(cultureCode);
        var settings = new JsonSerializerSettings() { Culture = culture };

        var rdr = new RecordedTransactionsReader(settings);
        var wrtr = new JsonWriter<RecordedTransaction>(settings);


        await using var writeStream = new MemoryStream();
        await using var writer = new StreamWriter(writeStream);

        await wrtr.Write(writer, RecordedTransactions, CancellationToken.None);
        await using var readStream = new MemoryStream(writeStream.ToArray());

        using var reader = new StreamReader(readStream);

        var result = await rdr.Read(reader, CancellationToken.None);

        result.ToList().Should().BeEquivalentTo(RecordedTransactions);
    }

    [Theory, UnitTest]
    [InlineData("ru-RU")]
    [InlineData("en-US")]
    public async Task SerializeAndDeserializeTransferKeysProperly(string cultureCode)
    {
        var culture = CultureInfo.GetCultureInfo(cultureCode);
        var settings = new JsonSerializerSettings() { Culture = culture };

        var rdr = new TransfersKeyReader(settings);
        var wrtr = new JsonWriter<TransferKey>(settings);

        await using var writeStream = new MemoryStream();
        await using var writer = new StreamWriter(writeStream);

        await wrtr.Write(writer, TransferKeys, CancellationToken.None);
        await using var readStream = new MemoryStream(writeStream.ToArray());

        using var reader = new StreamReader(readStream);

        var result = await rdr.Read(reader, CancellationToken.None);

        result.ToList().Should().BeEquivalentTo(TransferKeys);
    }

    [Theory, UnitTest]
    [InlineData("ru-RU")]
    [InlineData("en-US")]
    public async Task SerializeAndDeserializeExchangeRatesProperly(string cultureCode)
    {
        var culture = CultureInfo.GetCultureInfo(cultureCode);
        var settings = new JsonSerializerSettings() { Culture = culture };

        var rdr = new ExchangeRateReader(settings);
        var wrtr = new JsonWriter<ExchangeRate>(settings);


        await using var stream = new MemoryStream();
        await using var writer = new StreamWriter(stream);

        await wrtr.Write(writer, Rates, CancellationToken.None);
        stream.Seek(0, SeekOrigin.Begin);

        using var reader = new StreamReader(stream);

        var result = await rdr.Read(reader, CancellationToken.None);

        result.ToList().Should().BeEquivalentTo(Rates);
    }
}
