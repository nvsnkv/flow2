using System;
using Flow.Domain.Common.Collections;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

namespace Flow.Domain.Common.UnitTests;

public class EnumerableWithCountsShould
{
    private readonly int[] collection = { 1, 2, 4 };

    [Fact, UnitTest]
    public void IndicateThatCollectionWasNotEnumerated()
    {
        var stats = new EnumerableWithCount<int>(collection);
        stats.Enumerated.Should().BeFalse();
    }

    [Fact, UnitTest]
    public void IndicateThatCollectionWasEnumerated()
    {
        var stats = new EnumerableWithCount<int>(collection);
        foreach (var _ in stats)
        {
        }

        stats.Enumerated.Should().BeTrue();
    }

    [Fact, UnitTest]
    public void ThrowInvalidOperationExceptionIfCountRequestedBeforeCollectionWasEnumerated()
    {
        var stats = new EnumerableWithCount<int>(collection);
        Func<int> f = () => stats.Count;

        f.Should().Throw<InvalidOperationException>();
    }

    [Fact, UnitTest]
    public void CountCollectionProperly()
    {
        var stats = new EnumerableWithCount<int>(collection);
        foreach (var _ in stats)
        {
        }

        stats.Count.Should().Be(collection.Length);
    }
}