using FluentAssertions;
using Xunit;
using Xunit.Categories;

namespace Flow.Domain.Patterns.UnitTests;

public class PatternBuilderShould
{
    [Theory] [UnitTest]
    [InlineData(3, true)]
    [InlineData(null, false)]
    public void BuildSimpleExpressions(object? value, bool expected)
    {
        
        var pattern = new PatternBuilder<object?>().With(o => o != null).Build();
        var actual = pattern.Compile()(value);

        actual.Should().Be(expected);
    }

    [Theory] [UnitTest]
    [InlineData(5, 5, "", true)]
    [InlineData(5, 5, "Another text", true)]
    [InlineData(5, 3, "Another text", false)]
    public void BuildExpressionsWithComplexConditions(int expectedA, int a, string b, bool expected)
    {
        var pattern = new PatternBuilder<TestData>().With(t => t.A, ta => ta == expectedA).Build();
        var actual = pattern.Compile()(new TestData(a, b));

        actual.Should().Be(expected);
    }

    [Fact] [UnitTest]
    public void BuildExpressionsWithMultipleConditions()
    {
        var pattern = new PatternBuilder<TestData?>()
            .With(t => t != null)
            .With(t => t == null? 0 : t.A, ta => ta == 5)
            .With(t => t == null? "" : t.B, tb => !string.IsNullOrEmpty(tb))
            .With(t => t == null ? "" : t.B, tb => tb.StartsWith("B"))
            .Build();

        pattern.Compile()(new TestData(5, "Best string ever")).Should().BeTrue();
    }

    [Fact]
    [UnitTest]
    public void BuildExpressionsWithMultipleConditionsOnBaseTypes()
    {
        var pattern = new PatternBuilder<object>()
            .With(t => t != new object())
            .With(t => t.ToString(), ta => ta!.Length > 0)
            .With(t => t.ToString(), ta => ta!.Length < 1000)
            .Build();

        pattern.Compile()(new TestData(5, "Best string ever")).Should().BeTrue();
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