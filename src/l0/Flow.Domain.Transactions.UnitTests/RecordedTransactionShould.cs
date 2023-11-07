using System;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

namespace Flow.Domain.Transactions.UnitTests
{
    public class RecordedTransactionShould 
    {
        private readonly Transaction transaction = new(DateTime.UtcNow, -100, "RUB", null, "Transfer", new AccountInfo("name", "bank"));
        private readonly Transaction other = new(DateTime.UtcNow, - 10, "RUB", null, "Transfer", new AccountInfo("name", "bank"));

        [Fact, UnitTest]
        public void NotIgnoreTransactionDetailsInComparison()
        { 
            var rec = new RecordedTransaction(1, transaction, string.Empty);
            var oth = new RecordedTransaction(1, other, string.Empty);

            oth.Should().NotBe(rec);
        }

        [Fact, UnitTest]
        public void IgnoreOverridesInComparison()
        { 
            var rec = new RecordedTransaction(1, transaction, string.Empty) {
                Overrides = new(null, "Title", null)
            };

            var oth = new RecordedTransaction(1, transaction, string.Empty);
            var another = new RecordedTransaction(1, transaction, string.Empty) {
                Overrides = new Overrides("Overriden category", null, null)
            };

            oth.Should().Be(rec);
            another.Should().Be(rec);
        }

        [Fact, UnitTest]
        public void IgnoreRevisionInComparison()
        {
            var rec = new RecordedTransaction(1, transaction, Guid.NewGuid().ToString()) {
                Overrides = new(null, "Title", null)
            };

            var oth = new RecordedTransaction(1, transaction, Guid.NewGuid().ToString());
            var another = new RecordedTransaction(1, transaction, Guid.NewGuid().ToString()) {
                Overrides = new Overrides("Overriden category", null, null)
            };

            oth.Should().Be(rec);
            another.Should().Be(rec);
        }

        [Fact, UnitTest]
        public void NotBeEqualToRecordedTransactionWithDifferentKey() 
        {
            var rec = new RecordedTransaction(1, transaction, string.Empty);
            var oth = new RecordedTransaction(2, new Transaction(transaction), string.Empty);

            oth.Should().NotBe(rec);
        }

        [Fact, UnitTest]
        public void BeEqualWithTransactionWithSameTransactionDetails() 
        {
            var rec = new RecordedTransaction(1, transaction, string.Empty);
            rec.Equals(new Transaction(transaction)).Should().BeTrue();    
        }
    } 
}
