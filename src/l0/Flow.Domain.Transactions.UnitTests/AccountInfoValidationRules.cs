using Flow.Domain.Transactions.Validation;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

namespace Flow.Domain.Transactions.UnitTests;

public class AccountInfoValidationRulesShould
{
    [Fact, UnitTest]
    public void AcceptValidAccounts()
    {
        AccountInfoValidationRules.Check(new AccountInfo("name", "bank")).Should().BeTrue();
    }

    [Theory, UnitTest]
    [InlineData(null, "bank")]
    [InlineData(null, null)]
    [InlineData("account", null)]
    [InlineData("", "bank")]
    [InlineData("", "")]
    [InlineData("account", "")]
    public void RejectInvalidAccounts(string name, string bank)
    {
        AccountInfoValidationRules.Check(new AccountInfo(name, bank)).Should().BeFalse();
    }
}