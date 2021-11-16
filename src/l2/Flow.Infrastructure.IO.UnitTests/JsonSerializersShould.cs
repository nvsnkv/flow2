using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flow.Infrastructure.IO.Json;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;
using Xunit.Categories;

namespace Flow.Infrastructure.IO.UnitTests;

public class JsonSerializersShould : TestDataCarrier
{
    [Theory] [UnitTest]
    [InlineData("ru-RU")]
    [InlineData("en-US")]
    public async Task SerializeAndDeserializeTransactionsProperly(string cultureCode)
    {
        var culture = CultureInfo.GetCultureInfo(cultureCode);
        var serializer = new JsonTransactionsSerializer(new JsonSerializerSettings() { Culture = culture });

        await using var writeStream = new MemoryStream();
        await using var writer = new StreamWriter(writeStream);

        await serializer.WriteTransactions(writer, Transactions, CancellationToken.None);
        await using var readStream = new MemoryStream(writeStream.ToArray());
        
        using var reader = new StreamReader(readStream);

        var result = await serializer.ReadTransactions(reader);

        result.ToList().Should().BeEquivalentTo(Transactions);
    }

    [Theory] [UnitTest]
    [InlineData("ru-RU")]
    [InlineData("en-US")]
    public async Task SerializeAndDeserializeRecordedTransactionsProperly(string cultureCode)
    {
        var culture = CultureInfo.GetCultureInfo(cultureCode);
        var serializer = new JsonTransactionsSerializer(new JsonSerializerSettings { Culture = culture,  });

        await using var writeStream = new MemoryStream();
        await using var writer = new StreamWriter(writeStream);

        await serializer.WriteRecordedTransactions(writer, RecordedTransactions, CancellationToken.None);
        await using var readStream = new MemoryStream(writeStream.ToArray());

        using var reader = new StreamReader(readStream);

        var result = await serializer.ReadRecordedTransactions(reader);

        result.ToList().Should().BeEquivalentTo(RecordedTransactions);
    }

    [Theory] [UnitTest]
    [InlineData("ru-RU")]
    [InlineData("en-US")]
    public async Task SerializeAndDeserializeTransferKeysProperly(string cultureCode)
    {
        var culture = CultureInfo.GetCultureInfo(cultureCode);
        var serializer = new JsonTransfersSerializer(new JsonSerializerSettings { Culture = culture, });

        await using var writeStream = new MemoryStream();
        await using var writer = new StreamWriter(writeStream);

        await serializer.WriteTransferKeys(writer, TransferKeys, CancellationToken.None);
        await using var readStream = new MemoryStream(writeStream.ToArray());

        using var reader = new StreamReader(readStream);

        var result = await serializer.ReadTransferKeys(reader);

        result.ToList().Should().BeEquivalentTo(TransferKeys);
    }
}