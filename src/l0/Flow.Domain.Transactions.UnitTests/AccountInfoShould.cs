using FluentAssertions;
using Xunit;
using Xunit.Categories;

namespace Flow.Domain.Transactions.UnitTests
{
    public class AccountInfoShould
    {
        [Fact, UnitTest]
        public void BeEqualToAccountWithSameNameAndBank()
        {
            var account = new AccountInfo("The name", "The bank");
            var other = new AccountInfo("The name", "The bank");

            other.Should().Be(account);
        }
    
        [Theory, UnitTest]
        [InlineData("", "")]
        [InlineData("The name", "Another Bank")]
        [InlineData("Another name", "The bank")]
        [InlineData("Another name", "Another bank")]
        public void NotBeEqualToAccountWithDifferentProps(string otherName, string otherBank) 
        {
            var account = new AccountInfo("The name", "The bank");
            var other = new AccountInfo(otherName, otherBank);

            other.Should().NotBe(account);
        }
    }
}