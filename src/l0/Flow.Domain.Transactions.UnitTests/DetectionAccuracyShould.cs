using Flow.Domain.Transactions.Transfers;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

namespace Flow.Domain.Transactions.UnitTests;

public class DetectionAccuracyShould
{
    [Theory, UnitTest]
    [InlineData(DetectionAccuracy.Likely, DetectionAccuracy.Likely, 0)]
    [InlineData(DetectionAccuracy.Likely, DetectionAccuracy.Exact, -1)]
    [InlineData(DetectionAccuracy.Exact, DetectionAccuracy.Exact, 0)]
    [InlineData(DetectionAccuracy.Exact, DetectionAccuracy.Likely, 1)]
    public void BeComparable(DetectionAccuracy left, DetectionAccuracy right, int result)
    {
        switch (result)
        {
            case -1:
                (left < right).Should().BeTrue();
                break;
            case 0:
                (left == right).Should().BeTrue();
                break;

            case 1: 
                (left > right).Should().BeTrue();
                break;
        }
    }
}