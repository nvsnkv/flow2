using System;
using Flow.Application.ExchangeRates.Contract;
using Flow.Application.Transactions.Transfers;
using Flow.Domain.Transactions;
using FluentAssertions;
using Moq;
using Xunit;
using Xunit.Categories;

namespace Flow.Application.Transactions.UnitTests;

public class FuzzyTransferDetectorShould
{
    [Fact, UnitTest]
    public void NotThreatPairAsTransferIfSourceIsPositive()
    {
        var left = new RecordedTransaction(0, DateTime.UtcNow, 100, "ZBW", null, "Title", new("Account", "Bank"));
        var right = new RecordedTransaction(1, DateTime.UtcNow, 100, "ZBW", null, "Title", new("Account", "Bank"));

        new FuzzyTransferDetector(new Mock<IExchangeRatesProvider>().Object).CheckIsTransfer(left, right).Should().BeFalse();
    }

    [Fact, UnitTest]
    public void NotThreatPairAsTransferIfSourceDoesNotMatchSink()
    {
        var left = new RecordedTransaction(0, DateTime.UtcNow, -100, "ZBW", null, "Title", new("Account", "Bank"));
        var right = new RecordedTransaction(1, DateTime.UtcNow, 105, "ZBW", null, "Title", new("Account", "Bank"));

        new FuzzyTransferDetector(new Mock<IExchangeRatesProvider>().Object).CheckIsTransfer(left, right).Should().BeFalse();
    }

    [Fact, UnitTest]
    public void NotThreatPairAsTransferIfCurrencyDiffers()
    {
        var left = new RecordedTransaction(0, DateTime.UtcNow, -100, "ZBW", null, "Title", new("Account", "Bank"));
        var right = new RecordedTransaction(1, DateTime.UtcNow, 100, "BYN", null, "Title", new("Account", "Bank"));

        new FuzzyTransferDetector(new Mock<IExchangeRatesProvider>().Object).CheckIsTransfer(left, right).Should().BeFalse();
    }

    [Theory, UnitTest]
    [InlineData("2022-01-30", 4)]
    [InlineData("2022-01-28", 5)]
    [InlineData("2022-01-28", 10)]
    [InlineData("2022-01-28", 1000)]
    public void NotThreatPairAsTransferIfTimespanIsGreaterThenThreeBusinessDays(string timespan, int offset)
    {
        var left = new RecordedTransaction(0, DateTime.Parse(timespan), -100, "ZBW", null, "Title", new("Account", "Bank"));
        var right = new RecordedTransaction(1, DateTime.Parse(timespan).AddDays(offset), 100, "ZBW", null, "Title", new("Account", "Bank"));

        new FuzzyTransferDetector(new Mock<IExchangeRatesProvider>().Object).CheckIsTransfer(left, right).Should().BeFalse();
    }

    [Theory, UnitTest]
    [InlineData("2022-01-31", "2022-01-31")]
    [InlineData("2022-01-31", "2022-01-30")]
    [InlineData("2022-01-31", "2022-01-29")]
    [InlineData("2022-01-30", "2022-01-30")]
    [InlineData("2022-01-30", "2022-01-29")]
    public void DetectTransfersMadeOverTheWeekend(string sourceTimestamp, string sinkTimestamp)
    {
        var left = new RecordedTransaction(0, DateTime.Parse(sourceTimestamp), -100, "ZBW", null, "Title", new("Account", "Bank"));
        var right = new RecordedTransaction(1, DateTime.Parse(sinkTimestamp), 100, "ZBW", null, "Title", new("Accounto", "Bank"));

        new FuzzyTransferDetector(new Mock<IExchangeRatesProvider>().Object).CheckIsTransfer(left, right).Should().BeTrue();
    }

    [Theory, UnitTest]
    [InlineData("2022-01-28", "2022-01-31")]
    [InlineData("2022-01-29", "2022-01-31")]
    [InlineData("2022-01-30", "2022-01-31")]
    [InlineData("2022-01-31", "2022-01-31")]
    [InlineData("2022-01-28", "2022-02-01")]
    [InlineData("2022-01-29", "2022-02-01")]
    [InlineData("2022-01-30", "2022-02-01")]
    [InlineData("2022-01-31", "2022-02-01")]
    [InlineData("2022-01-28", "2022-02-02")]
    [InlineData("2022-01-29", "2022-02-02")]
    [InlineData("2022-01-30", "2022-02-02")]
    [InlineData("2022-01-31", "2022-02-02")]
    [InlineData("2022-01-31", "2022-02-03")]
    public void DetectTransfersMadeWithinThreeBusinessDays(string sourceTimestamp, string sinkTimestamp)
    {
        var left = new RecordedTransaction(0, DateTime.Parse(sourceTimestamp), -100, "ZBW", null, "Title", new("Account", "Bank"));
        var right = new RecordedTransaction(1, DateTime.Parse(sinkTimestamp), 100, "ZBW", null, "Title", new("El Accounto", "El Banco"));

        new FuzzyTransferDetector(new Mock<IExchangeRatesProvider>().Object).CheckIsTransfer(left, right).Should().BeTrue();
    }

    [Theory, UnitTest]
    [InlineData(null, "Transfer", null, "Transfer")]
    [InlineData(null, "Transfer", null, "Перевод")]
    [InlineData(null, "Transfer", "СБП", "Transfer")]
    [InlineData(null, "Transfer", "СБП", "Перевод")]
    public void DetectTransfersRegardlessOfItsNames(string? lc, string lt, string? rc, string rt)
    {
        var left = new RecordedTransaction(0, DateTime.UtcNow, -100, "ZBW", lc, lt, new("Account", "Bank"));
        var right = new RecordedTransaction(1, DateTime.UtcNow ,100, "ZBW", rc, rt, new("El Accounto", "El Banco"));

        new FuzzyTransferDetector(new Mock<IExchangeRatesProvider>().Object).CheckIsTransfer(left, right).Should().BeTrue();
    }
}