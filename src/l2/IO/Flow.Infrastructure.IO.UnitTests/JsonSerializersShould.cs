using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flow.Domain.ExchangeRates;
using Flow.Domain.Transactions;
using Flow.Domain.Transactions.Transfers;
using Flow.Infrastructure.IO.ExchangeRates;
using Flow.Infrastructure.IO.Transactions;
using Flow.Infrastructure.IO.Transactions.Transfers;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;
using Xunit.Categories;
using JsonSerializer = Flow.Infrastructure.IO.Generics.JsonSerializer;

namespace Flow.Infrastructure.IO.UnitTests;

public class JsonSerializersShould : TestDataCarrier
{
    [Theory, UnitTest]
    [InlineData("ru-RU")]
    [InlineData("en-US")]
    public async Task SerializeAndDeserializeTransactionsProperly(string cultureCode)
    {
        var culture = CultureInfo.GetCultureInfo(cultureCode);
        var serializer = new JsonSerializer(new JsonSerializerSettings() { Culture = culture });

        await using var writeStream = new MemoryStream();
        await using var writer = new StreamWriter(writeStream);

        await serializer.Write(writer, Transactions, CancellationToken.None);
        await using var readStream = new MemoryStream(writeStream.ToArray());
        
        using var reader = new StreamReader(readStream);

        var result = await serializer.Read(reader, (JsonTransaction j) => (Transaction)j);

        result.ToList().Should().BeEquivalentTo(Transactions);
    }

    [Theory, UnitTest]
    [InlineData("ru-RU")]
    [InlineData("en-US")]
    public async Task SerializeAndDeserializeRecordedTransactionsProperly(string cultureCode)
    {
        var culture = CultureInfo.GetCultureInfo(cultureCode);
        var serializer = new JsonSerializer(new JsonSerializerSettings { Culture = culture,  });

        await using var writeStream = new MemoryStream();
        await using var writer = new StreamWriter(writeStream);

        await serializer.Write(writer, RecordedTransactions, CancellationToken.None);
        await using var readStream = new MemoryStream(writeStream.ToArray());

        using var reader = new StreamReader(readStream);

        var result = await serializer.Read(reader, (JsonRecordedTransaction j) => (RecordedTransaction)j);

        result.ToList().Should().BeEquivalentTo(RecordedTransactions);
    }

    [Theory, UnitTest]
    [InlineData("ru-RU")]
    [InlineData("en-US")]
    public async Task SerializeAndDeserializeTransferKeysProperly(string cultureCode)
    {
        var culture = CultureInfo.GetCultureInfo(cultureCode);
        var serializer = new JsonSerializer(new JsonSerializerSettings { Culture = culture, });

        await using var writeStream = new MemoryStream();
        await using var writer = new StreamWriter(writeStream);

        await serializer.Write(writer, TransferKeys, CancellationToken.None);
        await using var readStream = new MemoryStream(writeStream.ToArray());

        using var reader = new StreamReader(readStream);

        var result = await serializer.Read(reader, (JsonTransferKey j) => (TransferKey)j);

        result.ToList().Should().BeEquivalentTo(TransferKeys);
    }

    [Theory, UnitTest]
    [InlineData("ru-RU")]
    [InlineData("en-US")]
    public async Task SerializeAndDeserializeExchangeRatesProperly(string cultureCode)
    {
        var culture = CultureInfo.GetCultureInfo(cultureCode);
        var serializer = new JsonSerializer(new JsonSerializerSettings { Culture = culture });

        await using var stream = new MemoryStream();
        await using var writer = new StreamWriter(stream);

        await serializer.Write(writer, Rates, CancellationToken.None);
        stream.Seek(0, SeekOrigin.Begin);

        using var reader = new StreamReader(stream);

        var result = await serializer.Read(reader, (JsonExchangeRate j) => (ExchangeRate)j);

        result.ToList().Should().BeEquivalentTo(Rates);
    }
}