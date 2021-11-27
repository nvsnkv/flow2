using System;
using System.Linq.Expressions;
using Flow.Domain.Patterns.Logical;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

namespace Flow.Domain.Patterns.UnitTests;

public class AndOperatorShould
{
    [Theory, UnitTest]
    [InlineData(true, true, true)]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    [InlineData(false, false, false)]
    public void ImplementLogicalAnd(bool left, bool right, bool expected)
    {
        Expression <Func<object, bool>> l = _ => left;
        Expression<Func<object, bool>> r = _ => right;

        var pattern = l.And(r);
        
        var actual = pattern.Compile()(new object());
        actual.Should().Be(expected);
    }

    [Fact, UnitTest]
    public void ChainSeveralExpressions()
    {
        Expression<Func<object, bool>> a = s => true;
        Expression<Func<object, bool>> b = s => true;
        Expression<Func<object, bool>> c = s => true;

        a.And(b).And(c).Compile()(new object()).Should().BeTrue();
    }
}