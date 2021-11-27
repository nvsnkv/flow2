using System;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

namespace Flow.Domain.ExchangeRates.UnitTests;

public class ExchangeRateRequestShould  
{
    [Fact] [UnitTest]
    public void BeEqualWithSameRequest()
    {
        var now = DateTime.UtcNow;
        var expected = ("RUB", "ZWL", now);
        var actual = ("RUB", "ZWL", now);

        actual.Should().Be(expected);
    }

    [Theory] [UnitTest]
    [InlineData("BLR", "ZWL", "2021-11-13")]
    [InlineData("RUB", "EUR", "2021-11-13")]
    [InlineData("RUB", "ZWL", "2021-11-11")]
    [InlineData("BLR", "EUR", "2021-11-13")]
    [InlineData("BLR", "ZWL", "2021-11-11")]
    [InlineData("BLR", "EUR", "2021-11-11")]
    public void BeNotEqualToDifferentRequests(string from, string to, string dateTime)
    {
        var expected = ("RUB", "ZWL", DateTime.Parse("2021-11-13"));
        var actual = (from, to, DateTime.Parse(dateTime));

        expected.Should().NotBe(actual);
    }
}