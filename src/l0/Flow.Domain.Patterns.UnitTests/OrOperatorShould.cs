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
        var l = new ExpressionPattern<object>(_ => left);
        var r = new ExpressionPattern<object>(_ => right);

        var pattern = l.Or(r);
        pattern.Should().BeOfType(typeof(OrPattern<object>));

        var actual = pattern.Match(new object());
        actual.Should().Be(expected);
    }
}