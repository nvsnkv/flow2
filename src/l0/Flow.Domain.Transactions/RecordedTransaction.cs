using System;

namespace Flow.Domain.Transactions;

public class RecordedTransaction: Transaction
{
    public RecordedTransaction(long key, DateTime timestamp, decimal amount, string currency, string? category, string title, AccountInfo account, string revision) : base(timestamp, amount, currency, category, title, account)
    {
        Key = key;
        Revision = revision;
    }

    public RecordedTransaction(long key, Transaction t, string revision) : base(t)
    {
        Key = key;
        Revision = revision;
    }

    public long Key { get; }

    public string Revision { get; }

    public Overrides? Overrides { get; set; }

    private bool Equals(RecordedTransaction other)
    {
        return Key == other.Key && base.Equals(other);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) { return true; }

        if (obj is RecordedTransaction other) { return Equals(other); }
        if (obj is Transaction otherTransaction) { return Equals(otherTransaction); }

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
