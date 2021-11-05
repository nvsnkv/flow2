using Flow.Domain.Patterns.Logical;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

namespace Flow.Domain.Patterns.UnitTests;

public class OrOperatorShould
{
    [Theory]
    [UnitTest]
    [InlineData(true, true, true)]
    [InlineData(true, false, true)]
    [InlineData(false, true, true)]
    [InlineData(false, false, false)]
    public void ImplementLogicalOr(bool left, bool right, bool expected)
    {
        var l = left ? Constants<object>.Truth : Constants<object>.Falsity;
        var r = right ? Constants<object>.Truth : Constants<object>.Falsity;

        var pattern = l.Or(r);

        var actual = pattern.Compile()(new object());
        actual.Should().Be(expected);
    }
}