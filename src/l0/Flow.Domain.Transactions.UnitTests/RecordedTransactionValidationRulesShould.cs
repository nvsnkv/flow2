using System;
using Flow.Domain.Transactions.Validation;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

namespace Flow.Domain.Transactions.UnitTests;

public class RecordedTransactionValidationRulesShould
{
    [Theory, UnitTest]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Category")]
    public void AcceptValidTransaction(string? category)
    {
        RecordedTransactionValidationRules.CheckRecordedTransaction(new RecordedTransaction(1, DateTime.UtcNow, -10, "EUR", category, "Title", new AccountInfo("name", "bank"), string.Empty)).Should().BeTrue();
    }

    [Theory, UnitTest]
    [InlineData(null, -10, "EUR", null, "Title")]
    [InlineData("2021-11-05", 0, "EUR", null, "Title")]
    [InlineData("2021-11-05", -10, "", null, "Title")]
    [InlineData("2021-11-05", -10, null, null, "Title")]
    [InlineData("2021-11-05", -10, "EUR", null, null)]
    [InlineData("2021-11-05", -10, "EUR", null, "")]
    public void RejectInvalidTransactionWhenTransactionRecordIsInvalid(string? timestamp, decimal amount, string currency, string? category, string title)
    {
        var ts = timestamp == null ? default : DateTime.Parse(timestamp);
        RecordedTransactionValidationRules.CheckRecordedTransaction(new RecordedTransaction(1, ts, -amount, currency, category, title, new AccountInfo("name", "bank"), string.Empty)).Should().BeFalse();
    }

    [Fact, UnitTest]
    public void RejectInvalidTransactionWhenAccountInfoIsInvalid()
    {
        RecordedTransactionValidationRules.CheckRecordedTransaction(new RecordedTransaction(1, DateTime.UtcNow, -10, "EUR", null, "Title", new AccountInfo("name", ""), string.Empty)).Should().BeFalse();
    }

    [Fact, UnitTest]
    public void RejectInvalidTransactionWhenKeyIsInvalid()
    {
        RecordedTransactionValidationRules.CheckRecordedTransaction(new RecordedTransaction(default, DateTime.UtcNow, -10, "EUR", null, "Title", new AccountInfo("name", "bank"), string.Empty)).Should().BeFalse();
    }
}
