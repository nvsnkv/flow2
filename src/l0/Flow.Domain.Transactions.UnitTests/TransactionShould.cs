using System;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

namespace Flow.Domain.Transactions.UnitTests
{
    public class TransactionShould 
    {
        [Fact] [UnitTest]
        public void BeEqualToAnotherTransactionWithSameAttributes()
        {
            var now = DateTime.UtcNow;
            var transaction = new Transaction(now, -100, "RUB", null, "Transfer", new AccountInfo("name", "bank"));
            var other = new Transaction(now,-100, "RUB", null, "Transfer", new AccountInfo("name", "bank"));

            other.Should().Be(transaction);
        }

        [Theory] [UnitTest]
        [InlineData("2021-11-06", -100, "RUB", null, "Transfer")]
        [InlineData("2021-11-05", -100, "RUB", null, "Not a Transfer")]
        [InlineData("2021-11-05", -100, "RUB", "Category", "Transfer")]
        [InlineData("2021-11-05", -100, "EUR", null, "Transfer")]
        [InlineData("2021-11-05", 100, "RUB", null, "Not a Transfer")]
        public void NotBeEqualToDifferentTransaction(string timestamp, decimal amount, string currency, string? category, string title) 
        {
            var account = new AccountInfo("name", "bank");
            var transaction = new Transaction(DateTime.Parse("2021-11-05"), -100, "RUB", null, "Transfer", account);
            var other = new Transaction(DateTime.Parse(timestamp), amount, currency, category, title, account);

            other.Should().NotBe(transaction);
        }

        [Fact] [UnitTest]
        public void NotBeEqualToTransactionWithDifferentAccount() 
        {
            var now = DateTime.UtcNow;
            var transaction = new Transaction(now, -100, "RUB", null, "Transfer", new AccountInfo("name", "bank"));
            var other = new Transaction(now, -100, "RUB", null, "Transfer", new AccountInfo("name", " another bank"));

            other.Should().NotBe(transaction);
        }
    }
}