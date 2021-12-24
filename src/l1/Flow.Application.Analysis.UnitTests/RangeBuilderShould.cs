using Flow.Domain.Analysis;
using FluentAssertions;
using Xunit;
using Xunit.Categories;
using Range = Flow.Domain.Analysis.Range;

namespace Flow.Application.Analysis.UnitTests;

public class RangeBuilderShould
{
    [Fact, UnitTest]
    public void Produce12OffsetsForYear()
    {
        var expected = new[]
        {
            new Range(DateTime.Parse("2021-01-01").ToUniversalTime(), DateTime.Parse("2021-02-01").ToUniversalTime()),
            new Range(DateTime.Parse("2021-02-01").ToUniversalTime(), DateTime.Parse("2021-03-01").ToUniversalTime()),
            new Range(DateTime.Parse("2021-03-01").ToUniversalTime(), DateTime.Parse("2021-04-01").ToUniversalTime()),
            new Range(DateTime.Parse("2021-04-01").ToUniversalTime(), DateTime.Parse("2021-05-01").ToUniversalTime()),
            new Range(DateTime.Parse("2021-05-01").ToUniversalTime(), DateTime.Parse("2021-06-01").ToUniversalTime()),
            new Range(DateTime.Parse("2021-06-01").ToUniversalTime(), DateTime.Parse("2021-07-01").ToUniversalTime()),
            new Range(DateTime.Parse("2021-07-01").ToUniversalTime(), DateTime.Parse("2021-08-01").ToUniversalTime()),
            new Range(DateTime.Parse("2021-08-01").ToUniversalTime(), DateTime.Parse("2021-09-01").ToUniversalTime()),
            new Range(DateTime.Parse("2021-09-01").ToUniversalTime(), DateTime.Parse("2021-10-01").ToUniversalTime()),
            new Range(DateTime.Parse("2021-10-01").ToUniversalTime(), DateTime.Parse("2021-11-01").ToUniversalTime()),
            new Range(DateTime.Parse("2021-11-01").ToUniversalTime(), DateTime.Parse("2021-12-01").ToUniversalTime()),
            new Range(DateTime.Parse("2021-12-01").ToUniversalTime(), DateTime.Parse("2022-01-01").ToUniversalTime()),
        };

        var actual = new MonthlyRangesBuilder(DateTime.Parse("2021-01-01").ToUniversalTime(), DateTime.Parse("2022-01-01").ToUniversalTime()).GetRanges();

        actual.Should().BeEquivalentTo(expected, c => c.Excluding(r => r.Alias));
    }
}