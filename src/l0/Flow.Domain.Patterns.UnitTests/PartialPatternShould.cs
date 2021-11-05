using FluentAssertions;
using Xunit;
using Xunit.Categories;

namespace Flow.Domain.Patterns.UnitTests;

public class PartialPatternShould
{

    [Theory] [UnitTest]
    [InlineData(5, 5,"", true)]
    [InlineData(5, 5, "Another text", true)]
    [InlineData(5, 3, "Another text", false)]
    public void MatchOnlyWhenObjectSatisfiesCondition(int expectedA, int a, string b, bool expected)
    {
        var pattern = new PartialPattern<TestData, int>(t => t.A, new ExpressionPattern<int>(ta => ta == expectedA));
        var actual = pattern.Match(new TestData(a, b));

        actual.Should().Be(expected);
    }

    [Fact] [UnitTest]
    public void BeConvertibleFromTuple()
    {
        bool TestMatch<T, TA>(PartialPattern<T, TA> pattern, T obj)
        {
            return pattern.Match(obj);
        }

        var actual = TestMatch<TestData, int>((t => t.A, new ExpressionPattern<int>(a => a == 10)), new TestData(10, "V"));
        actual.Should().Be(true);
    }


    private sealed class TestData
    {
        public TestData(int a, string b)
        {
            A = a;
            B = b;
        }

        public int A { get; }

        public string B { get; }
    }
}