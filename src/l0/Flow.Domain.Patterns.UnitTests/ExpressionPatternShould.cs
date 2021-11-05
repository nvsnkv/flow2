using System;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

namespace Flow.Domain.Patterns.UnitTests;

public class ExpressionPatternShould
{
    [Theory] [UnitTest]
    [InlineData(3, true)]
    [InlineData(null, false)]
    public void MatchIfExpressionReturnsTrue(object? value, bool expected)
    {
        var pattern = new ExpressionPattern<object?>(o => o != null);
        var actual = pattern.Match(value);

        actual.Should().Be(expected);
    }

    [Fact]
    [UnitTest]
    public void BeConvertibleFromExpression()
    {
        ExpressionPattern<object?> pattern = (Expression<Func<object?, bool>>)(o => o != null);
        
        pattern.Match(new object()).Should().Be(true);
    }
}