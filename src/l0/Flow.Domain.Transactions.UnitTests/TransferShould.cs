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

    [Fact] [UnitTest]
    public void ThrowArgumentExceptionIfSourceAndSinkAreTheSame()
    {
        Action a = () =>
        {
            var _ = new Transfer(0, 0, 0, "");
        };

        a.Should().Throw<ArgumentException>().Where(e => e.ParamName == "sink");
    }

    [Fact] [UnitTest]
    public void ThrowArgumentExceptionIfTransactionsAreInDifferentCurrencies()
    {
        Action a = () =>
        {
            var _ = new Transfer(source, sinkWithDifferentCurrency);
        };

        a.Should().Throw<ArgumentException>().Where(e => e.ParamName == "sink");
    }

    [Fact] [UnitTest]
    public void ThrowArgumentExceptionIfTransactionsIfSourceHasPositiveAmount()
    {
        Action a = () =>
        {
            var _ = new Transfer(sinkWithFee, sink);
        };

        a.Should().Throw<ArgumentException>().Where(e => e.ParamName == "source");
    }

    [Fact] [UnitTest]
    public void ThrowArgumentExceptionIfTransactionsIfSinkHasNegativeAmount()
    {
        Action a = () =>
        {
            var _ = new Transfer(source, anotherSource);
        };

        a.Should().Throw<ArgumentException>().Where(e => e.ParamName == "sink");
    }

    [Fact] [UnitTest]
    public void CalculateFeeAsDifferenceBetweenSourceAndSink()
    {
        var transfer = new Transfer(source, sinkWithFee);
        transfer.Fee.Should().Be(source.Amount + sinkWithFee.Amount);
        transfer.Currency.Should().Be(source.Currency);
    }

    [Fact] [UnitTest]
    public void PopulateSourceAndSinkProperly()
    {
        var transfer = new Transfer(source, sink);
        transfer.Source.Should().Be(source.Key);
        transfer.Sink.Should().Be(sink.Key);
    }
}