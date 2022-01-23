using System;
using Flow.Domain.Transactions.Transfers;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

namespace Flow.Domain.Transactions.UnitTests;

public class TransferShould
{
    private static readonly AccountInfo Account = new("The name", "The Bank");

    private readonly RecordedTransaction source = new(1, DateTime.UtcNow, -100, "RUR", null, "Transfer", Account);
    private readonly RecordedTransaction anotherSource = new(5, DateTime.UtcNow, -100, "RUR", null, "Transfer", Account);
    private readonly RecordedTransaction sink = new(2, DateTime.UtcNow, 100, "RUR", null, "Transfer", Account);
    private readonly RecordedTransaction sinkWithFee = new(3, DateTime.UtcNow, 90, "RUR", null, "Transfer", Account);
    private readonly RecordedTransaction sinkWithDifferentCurrency = new(4, DateTime.UtcNow, 90, "EUR", null, "Transfer", Account);

    [Fact, UnitTest]
    public void ThrowArgumentExceptionIfSourceAndSinkAreTheSame()
    {
        Action a = () =>
        {
            var _ = new Transfer(GetTransaction(1), GetTransaction(1), DetectionAccuracy.Exact);
        };

        a.Should().Throw<ArgumentException>().Where(e => e.ParamName == "sink");
    }

    [Fact, UnitTest]
    public void ThrowArgumentExceptionIfTransactionsAreInDifferentCurrencies()
    {
        Action a = () =>
        {
            var _ = new Transfer(source, sinkWithDifferentCurrency, DetectionAccuracy.Exact);
        };

        a.Should().Throw<ArgumentException>().Where(e => e.ParamName == "sink");
    }

    [Fact, UnitTest]
    public void ThrowArgumentExceptionIfTransactionsIfSourceHasPositiveAmount()
    {
        Action a = () =>
        {
            var _ = new Transfer(sinkWithFee, sink, DetectionAccuracy.Exact);
        };

        a.Should().Throw<ArgumentException>().Where(e => e.ParamName == "source");
    }

    [Fact, UnitTest]
    public void ThrowArgumentExceptionIfTransactionsIfSinkHasNegativeAmount()
    {
        Action a = () =>
        {
            var _ = new Transfer(source, anotherSource, DetectionAccuracy.Exact);
        };

        a.Should().Throw<ArgumentException>().Where(e => e.ParamName == "sink");
    }

    [Fact, UnitTest]
    public void CalculateFeeAsDifferenceBetweenSourceAndSink()
    {
        var transfer = new Transfer(source, sinkWithFee, DetectionAccuracy.Exact);
        transfer.Fee.Should().Be(source.Amount + sinkWithFee.Amount);
        transfer.Currency.Should().Be(source.Currency);
    }

    [Fact, UnitTest]
    public void PopulateSourceAndSinkProperly()
    {
        var transfer = new Transfer(source, sink, DetectionAccuracy.Exact);
        transfer.Source.Should().Be(source);
        transfer.Sink.Should().Be(sink);
    }

    [Theory, UnitTest]
    [InlineData(1, 2, 100, 100, "RUR", "RUR")]
    [InlineData(1, 2, 100, 200, "RUR", "RUR")]
    [InlineData(1, 2, 100, 100, "EUR", "RUR")]
    [InlineData(1, 2, 100, 200, "EUR", "RUR")]

    public void MatchIfKeysMatch(long source, long sink, decimal lFee, decimal rFee, string lCurrency, string rCurrency)
    {
        var src = GetTransaction(source);
        var snk = GetTransaction(sink, default, +15);
        var left = new Transfer(src, snk, DetectionAccuracy.Exact, lFee, lCurrency);
        var right = new Transfer(src, snk, DetectionAccuracy.Exact, rFee, rCurrency);

        left.Should().Be(right);
    }

    [Fact, UnitTest]
    public void HaveHashKeyDefinedByTransferKey()
    {
        var transfer = new Transfer(GetTransaction(100), GetTransaction(400, default, +15), DetectionAccuracy.Exact, 0, "RUR");
        var key = new TransferKey(100, 400);

        key.GetHashCode().Should().Be(transfer.GetHashCode());
    }

    [Theory, UnitTest]
    [InlineData(1, 2, 1, 3, 100, 100, "RUR", "RUR")]
    [InlineData(1, 2, 2, 1, 100, 200, "RUR", "RUR")]
    [InlineData(1, 2, 3, 4, 100, 100, "EUR", "RUR")]
    [InlineData(1, 2, 1, 3, 100, 200, "EUR", "RUR")]
    [InlineData(1, 2, 1, 3, 100, 200, "RUR", "RUR")]
    [InlineData(1, 2, 2, 1, 100, 100, "EUR", "RUR")]
    [InlineData(1, 2, 3, 4, 100, 200, "EUR", "RUR")]
    public void DifferIfKeysAreDifferent(long ls, long lsnk, long rs, long rsnk, decimal lFee, decimal rFee, string lCurrency, string rCurrency)
    {
        var left = new Transfer(GetTransaction(ls), GetTransaction(lsnk, default, +15), DetectionAccuracy.Exact, lFee, lCurrency);
        var right = new Transfer(GetTransaction(rs), GetTransaction(rsnk, default, +15), DetectionAccuracy.Exact, rFee, rCurrency);

        left.Should().NotBe(right);
    }

    private static RecordedTransaction GetTransaction(long key, DateTime timestamp = default, decimal amount = -350m,
        string currency = "ZBW", string title = "title")
    {
        return new RecordedTransaction(key, timestamp, amount, currency, null, title, new("Account", "Bank"));
    }
}