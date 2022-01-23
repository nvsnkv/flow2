using System;

namespace Flow.Domain.Transactions.Transfers;

public class TransferKey
{
    public TransferKey(long sourceKey, long sinkKey)
    {
        SourceKey = sourceKey;
        SinkKey = sinkKey;
    }

    public long SourceKey { get; }
    public long SinkKey { get; }

    protected bool Equals(TransferKey other)
    {
        return SourceKey == other.SourceKey && SinkKey == other.SinkKey;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (!(obj is TransferKey key)) return false;
        return Equals(key);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(SourceKey, SinkKey);
    }

    public static bool operator ==(TransferKey? left, TransferKey? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(TransferKey? left, TransferKey? right)
    {
        return !Equals(left, right);
    }
}