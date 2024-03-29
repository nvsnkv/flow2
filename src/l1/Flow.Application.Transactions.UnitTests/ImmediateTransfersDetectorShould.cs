using System;
using System.Threading;
using System.Threading.Tasks;
using Flow.Application.ExchangeRates.Contract;
using Flow.Application.Transactions.Transfers;
using Flow.Domain.Transactions;
using Flow.Domain.Transactions.Transfers;
using FluentAssertions;
using Moq;
using Xunit;
using Xunit.Categories;

namespace Flow.Application.Transactions.UnitTests;

public class ImmediateTransfersDetectorShould
{
    private readonly Mock<IExchangeRatesProvider> ratesProvider = new();

    [Theory, UnitTest]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Category")]
    public void DetectImmediateTransfers(string? category)
    {
        var now = DateTime.UtcNow;
        var source = new RecordedTransaction(1, now, -20, "ZWL", category, "Transfer", new AccountInfo("From", "bank"), Guid.NewGuid().ToString());
        var sink = new RecordedTransaction(2, now, 20, "ZWL", category, "Transfer", new AccountInfo("To", "bank"), Guid.NewGuid().ToString());

        new ImmediateTransfersDetector(ratesProvider.Object).CheckIsTransfer(source, sink).Should().BeTrue();
    }

    [Fact, UnitTest]
    public void CreateImmediateTransferFromTransaction()
    {
        var now = DateTime.UtcNow;
        var source = new RecordedTransaction(1, now, -20, "ZWL", null, "Transfer", new AccountInfo("From", "bank"), Guid.NewGuid().ToString());
        var sink = new RecordedTransaction(2, now, 20, "ZWL", null, "Transfer", new AccountInfo("To", "bank"), Guid.NewGuid().ToString());

        new ImmediateTransfersDetector(ratesProvider.Object).CheckIsTransfer(source, sink).Should().BeTrue();
    }

    [Theory, UnitTest]
    [InlineData(null, 0, null, null, "A")]
    [InlineData(null, 0, null, "A", null)]
    [InlineData(null, 0, "A", null, null)]
    [InlineData(null, 1.0, null, null, null)]
    [InlineData("00:02:01", 0, null, null, null)]
    [InlineData(null, 0, null, "A", "null")]
    [InlineData(null, 0, "A", null, "null")]
    [InlineData(null, 1.0, null, null, "null")]
    [InlineData("00:02:01", 0, null, null, "null")]
    [InlineData(null, 0, "A", "null", "null")]
    [InlineData(null, 1.0, null, "null", "null")]
    [InlineData("00:02:01", 0, null, "null", "null")]
    [InlineData(null, 1.0, "null", "null", "null")]
    [InlineData("00:02:01", 0, "null", "null", "null")]
    [InlineData("00:02:01", 2.5, "null", "null", "null")]
    [InlineData("-00:01:01", 2.5, "null", "null", "null")]
    public void IgnoreTransactionsThatDoesNotMeetImmediateTransferCriteria(string? timeShift, decimal amountShift, string? currencyShift, string? categoryShift, string? titleShift)
    {
        var now = DateTime.UtcNow;
        var source = new RecordedTransaction(1, now, -100, "ZWL", "C", "Transfer", new AccountInfo("From", "bank"), Guid.NewGuid().ToString());
        var sink = new RecordedTransaction(2,
            timeShift != null ? now.Add(TimeSpan.Parse(timeShift)) : now,
            source.Amount * -1 + amountShift,
            source.Currency + (currencyShift ?? ""),
            source.Category + (categoryShift ?? ""),
            source.Title + (titleShift ?? ""),
            new AccountInfo("To", "bank"),
            Guid.NewGuid().ToString()
            );

        new ImmediateTransfersDetector(ratesProvider.Object).CheckIsTransfer(source, sink).Should().BeFalse();
    }

    [Fact, UnitTest]
    public void IgnoreTransactionsThatHasFromAccount()
    {
        var now = DateTime.UtcNow;
        var source = new RecordedTransaction(1, now, -100, "ZWL", "C", "Transfer", new AccountInfo("From", "bank"), Guid.NewGuid().ToString());
        var sink = new RecordedTransaction(1, now, 100, "ZWL", "C", "Transfer", new AccountInfo("From", "bank"), Guid.NewGuid().ToString());

        new ImmediateTransfersDetector(ratesProvider.Object).CheckIsTransfer(source, sink).Should().BeFalse();
    }

    [Fact, UnitTest]
    public async Task SetAccuracyLevelToExact()
    {
        var now = DateTime.UtcNow;
        var source = new RecordedTransaction(1, now, -20, "ZWL", null, "Transfer", new AccountInfo("From", "bank"), Guid.NewGuid().ToString());
        var sink = new RecordedTransaction(2, now, 20, "ZWL", null, "Transfer", new AccountInfo("To", "bank"), Guid.NewGuid().ToString());

        var result = await new ImmediateTransfersDetector(ratesProvider.Object).Create(source, sink, CancellationToken.None);
        result.AccuracyLevel.Should().Be(DetectionAccuracy.Exact);
    }
}
