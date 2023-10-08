using System.Collections.Generic;
using System.Linq;
using Flow.Infrastructure.IO.Transactions.Criteria;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

namespace Flow.Infrastructure.IO.UnitTests;

public class CriteriaInputHandlerShould
{
    [Theory, UnitTest]
    [InlineData(new [] { "cat=1" }, new[] { "cat=1" })]
    [InlineData(new [] { "cat=1 'a<0'" }, new[] { "cat=1", "a<0" })]
    [InlineData(new [] { "t='title with whitespace'" }, new[] { "t=title with whitespace" })]
    [InlineData(new [] { @"t=""title in double quotes""" }, new[] { "t=title in double quotes" })]
    [InlineData(new [] { @"'t=""double-quoted""'" }, new[] { "t=double-quoted" })]
    public void CleanupItemsProperly(IEnumerable<string> input, string[] expected)
    {
        var result = CriteriaInputHandler.SplitAndUnquote(input);
        result.ToArray().Should().BeEquivalentTo(expected);
    }
}