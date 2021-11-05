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
            var transaction = new Transaction(-100, "RUB", null, "Transfer", new AccountInfo("name", "bank"));
            var other = new Transaction(-100, "RUB", null, "Transfer", new AccountInfo("name", "bank"));

            other.Should().Be(transaction);
        }

        [Theory] [UnitTest]
        [InlineData(-100, "RUB", null, "Not a Transfer")]
        [InlineData(-100, "RUB", "Category", "Transfer")]
        [InlineData(-100, "EUR", null, "Transfer")]
        [InlineData(100, "RUB", null, "Not a Transfer")]
        public void NotBeEqualToDifferentTransaction(decimal amount, string currency, string? category, string title) 
        {
            var account = new AccountInfo("name", "bank");
            var transaction = new Transaction(-100, "RUB", null, "Transfer", account);
            var other = new Transaction(amount, currency, category, title, account);

            other.Should().NotBe(transaction);
        }

        [Fact] [UnitTest]
        public void NotBeEqualToTransactionWithDifferentAccount() 
        {
            var transaction = new Transaction(-100, "RUB", null, "Transfer", new AccountInfo("name", "bank"));
            var other = new Transaction(-100, "RUB", null, "Transfer", new AccountInfo("name", " another bank"));

            other.Should().NotBe(transaction);
        }
    }
}