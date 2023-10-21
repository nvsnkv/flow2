using System.Globalization;
using Flow.Domain.Transactions;
using FluentAssertions;

namespace Flow.Plugins.Transactions.FlexibleTransactionsReader.Tests;

public class FlexibleTransactionReaderShould
{
    [Fact]
    public async Task ReadCorrectFileSuccessfully()
    {
        var expectedFormat = "valid-format"; // from JSON
        var culture = CultureInfo.GetCultureInfoByIetfLanguageTag("ru-RU");
        var bootstrapper = new Bootstrapper("./TestData/TestData_Valid_ExtraColumns.json", culture);
        var reader = bootstrapper.GetPlugins().Single() as IFlexibleTransactionsReader;

        using var streamReader = new StreamReader(File.OpenRead("./TestData/TestData_Valid_ExtraColumns.csv"));

        reader.Should().NotBeNull();
        reader!.Format.Name.Should().Be(expectedFormat);

        var transactions = await reader.Read(streamReader, CancellationToken.None);
        transactions.Select(p => p.Transaction).Should().BeEquivalentTo(TestData_Valid_ExtraColumns_Transactions);
    }

    private static readonly Transaction[] TestData_Valid_ExtraColumns_Transactions =
    {
        new(DateTime.Parse("18.10.2023 11:44:17"), -100, "RUB", "Transfers", "To someone", new AccountInfo("Test Account", "Test Bank")),
        new(DateTime.Parse("18.10.2023 11:50:17"), 100, "RUB", "Transfers", "From someone", new AccountInfo("Test Account", "Test Bank"))
    };
}
