using System;
using System.Linq;
using System.Threading.Tasks;
using Flow.Domain.Common.Collections.Async;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

namespace Flow.Domain.Common.UnitTests;

public class AsyncEnumerableWithCountsShould
{
    private readonly int[] collection = { 1, 2, 4 };

    [Fact] [UnitTest]
    public void IndicateThatCollectionWasNotEnumerated()
    {
        var stats = new AsyncEnumerableWithCount<int>(collection.ToAsyncEnumerable());
        stats.Enumerated.Should().BeFalse();
    }

    [Fact] [UnitTest]
    public async Task IndicateThatCollectionWasEnumerated()
    {
        var stats = new AsyncEnumerableWithCount<int>(collection.ToAsyncEnumerable());
        await foreach (var _ in stats)
        {
        }

        stats.Enumerated.Should().BeTrue();
    }

    [Fact] [UnitTest]
    public void ThrowInvalidOperationExceptionIfCountRequestedBeforeCollectionWasEnumerated()
    {
        var stats = new AsyncEnumerableWithCount<int>(collection.ToAsyncEnumerable());
        Func<int> f = () => stats.Count;

        f.Should().Throw<InvalidOperationException>();
    }

    [Fact] [UnitTest]
    public async Task CountCollectionProperly()
    {
        var stats = new AsyncEnumerableWithCount<int>(collection.ToAsyncEnumerable());
        await foreach (var _ in stats)
        {
        }

        stats.Count.Should().Be(collection.Length);
    }
}