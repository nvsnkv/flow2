using System;
using System.Linq;
using Flow.Domain.Transactions.Collections;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

namespace Flow.Domain.Transactions.UnitTests;

public class TransactionsWithDateRangeShould
{
    private readonly Transaction[] transactions =
    {
        new(DateTime.Parse("2021-11-09"), 100, "RUB", null, "Title", new AccountInfo("name", "bank")),
        new(DateTime.Parse("2021-11-10"), 100, "RUB", null, "Title", new AccountInfo("name", "bank")),
        new(DateTime.Parse("2021-11-11"), 100, "RUB", null, "Title", new AccountInfo("name", "bank"))
    };

    [Fact] [UnitTest]
    public void ThrowInvalidOperationExceptionIfCollectionWasNotEnumerated()
    {
        var stats = new TransactionsWithDateRange<Transaction>(transactions);

        Func<DateTime?> min = () => stats.Min;
        Func<DateTime?> max = () => stats.Max;

        min.Should().Throw<InvalidOperationException>();
        max.Should().Throw<InvalidOperationException>();
    }
    
    [Fact] [UnitTest]
    public void CountMinMaxTimestampValues()
    {
        var stats = new TransactionsWithDateRange<Transaction>(transactions);
        foreach (var _ in stats)
        {
        }

        stats.Enumerated.Should().BeTrue();
        stats.Min.Should().NotBeNull();
        stats.Max.Should().NotBeNull();

        stats.Min.Should().Be(transactions.Min(t => t.Timestamp));
        stats.Max.Should().Be(transactions.Max(t => t.Timestamp));
    }
}