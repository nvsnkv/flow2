using System;

namespace Flow.Domain.Transactions
{
    public sealed class RecordedTransaction: Transaction
    {
        public RecordedTransaction(long key, DateTime timestamp, decimal amount, string currency, string? category, string title, AccountInfo account) : base(timestamp, amount, currency, category, title, account)
        {
            Key = key;
        }

        public RecordedTransaction(long key, Transaction t) : base(t)
        {
            Key = key;
        }

        public long Key { get; }

        public Overrides? Overrides { get; set; }

        private bool Equals(RecordedTransaction other)
        {
            return Key == other.Key;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj)) { return true; }

            if (obj is RecordedTransaction other) { return Equals(other); }
            if (obj is Transaction otherTransaction) { return Equals(otherTransaction); };

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Key);
        }

        public static bool operator ==(RecordedTransaction? left, RecordedTransaction? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(RecordedTransaction? left, RecordedTransaction? right)
        {
            return !Equals(left, right);
        }
    }
}