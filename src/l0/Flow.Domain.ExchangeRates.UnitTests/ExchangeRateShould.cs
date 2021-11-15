using System;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

namespace Flow.Domain.ExchangeRates.UnitTests;

public class ExchangeRateShould
{
    [Fact]
    [UnitTest]
    public void BeEqualWithSameRates()
    {
        var now = DateTime.UtcNow;
        var expected = new ExchangeRate("RUB", "ZWL", now, 4.5M);
        var actual = new ExchangeRate("RUB", "ZWL", now, 4.5M);

        actual.Should().Be(expected);
    }

    [Theory]
    [UnitTest]
    [InlineData("BLR", "ZWL", "2021-11-13", 4.5)]
    [InlineData("RUB", "EUR", "2021-11-13", 4.5)]
    [InlineData("RUB", "ZWL", "2021-11-11", 4.5)]
    [InlineData("BLR", "EUR", "2021-11-13", 4.5)]
    [InlineData("BLR", "ZWL", "2021-11-11", 4.5)]
    [InlineData("BLR", "EUR", "2021-11-11", 4.5)]
    [InlineData("BLR", "ZWL", "2021-11-13", 5.0)]
    [InlineData("RUB", "EUR", "2021-11-13", 5.0)]
    [InlineData("RUB", "ZWL", "2021-11-11", 5.0)]
    [InlineData("BLR", "EUR", "2021-11-13", 5.0)]
    [InlineData("BLR", "ZWL", "2021-11-11", 5.0)]
    [InlineData("BLR", "EUR", "2021-11-11", 5.0)]
    public void BeNotEqualToDifferentRequests(string from, string to, string dateTime, decimal rate)
    {
        var expected = new ExchangeRate("RUB", "ZWL", DateTime.Parse("2021-11-13"), 4.5M);
        var actual = new ExchangeRate(from, to, DateTime.Parse(dateTime), rate);

        expected.Should().NotBe(actual);
    }
}