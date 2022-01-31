using System;
using FluentAssertions;
using FluentAssertions.Extensions;
using Xunit;
using Xunit.Categories;

namespace Flow.Domain.Common.UnitTests;

public class BusinessDateExtensionShould
{
    [Theory, UnitTest]
    [InlineData("2022-01-31T00:00:00.0000000Z", "2022-01-31T00:00:00.0000000Z")]
    [InlineData("2022-01-31T15:00:00.0000000Z", "2022-01-31T00:00:00.0000000Z")]
    [InlineData("2022-01-31T23:59:59.0000000Z", "2022-01-31T00:00:00.0000000Z")]
    [InlineData("2022-01-30T23:59:59.0000000Z", "2022-01-31T00:00:00.0000000Z")]
    [InlineData("2022-01-29T00:00:00.0000000Z", "2022-01-31T00:00:00.0000000Z")]
    public void ProvideExpectedBusinessDate(string incoming, string expected)
    {
        DateTime.Parse(incoming).ToUniversalTime().BusinessDate().Should().Be(DateTime.Parse(expected).ToUniversalTime());
    }
}