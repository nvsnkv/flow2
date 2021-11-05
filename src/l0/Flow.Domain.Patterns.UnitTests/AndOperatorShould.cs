using Flow.Domain.Patterns.Logical;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

namespace Flow.Domain.Patterns.UnitTests;

public class AndOperatorShould
{
    [Theory]
    [UnitTest]
    [InlineData(true, true, true)]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    [InlineData(false, false, false)]
    public void ImplementLogicalAnd(bool left, bool right, bool expected)
    {
        var l = new ExpressionPattern<object>(_ => left);
        var r = new ExpressionPattern<object>(_ => right);

        var pattern = l.And(r);
        pattern.Should().BeOfType(typeof(AndPattern<object>));

        var actual = pattern.Match(new object());
        actual.Should().Be(expected);
    }
}