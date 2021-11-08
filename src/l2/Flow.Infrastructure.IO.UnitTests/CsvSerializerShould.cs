using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using Flow.Infrastructure.IO.Csv;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

namespace Flow.Infrastructure.IO.UnitTests;

public class CsvSerializerShould : TestDataCarrier
{
    [Theory] [UnitTest]
    [InlineData("ru-RU")]
    [InlineData("en-US")]
    public async Task SerializeAndDeserializeTransactionsProperly(string cultureCode)
    {
        var culture = CultureInfo.GetCultureInfo(cultureCode);
        var serializer = new CsvTransactionsSerializer(new CsvConfiguration(culture) { LeaveOpen = true });

        await using var stream = new MemoryStream();
        await using var writer = new StreamWriter(stream);

        await serializer.WriteTransactions(writer, Transactions, CancellationToken.None);
        stream.Seek(0, SeekOrigin.Begin);

        using var reader = new StreamReader(stream);

        var result = await serializer.ReadTransactions(reader, CancellationToken.None);

        result.ToList().Should().BeEquivalentTo(Transactions);
    }

    [Theory] [UnitTest]
    [InlineData("ru-RU")]
    [InlineData("en-US")]
    public async Task SerializeAndDeserializeRecordedTransactionsProperly(string cultureCode)
    {
        var culture = CultureInfo.GetCultureInfo(cultureCode);
        var serializer = new CsvTransactionsSerializer(new CsvConfiguration(culture) { LeaveOpen = true });

        await using var stream = new MemoryStream();
        await using var writer = new StreamWriter(stream);

        await serializer.WriteRecordedTransactions(writer, RecordedTransactions, CancellationToken.None);
        stream.Seek(0, SeekOrigin.Begin);

        using var reader = new StreamReader(stream);

        var result = await serializer.ReadRecordedTransactions(reader, CancellationToken.None);

        result.ToList().Should().BeEquivalentTo(RecordedTransactions);
    }
}