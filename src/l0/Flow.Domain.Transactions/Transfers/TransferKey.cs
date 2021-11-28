using System;

namespace Flow.Domain.Transactions.Transfers;

public class TransferKey
{
    public TransferKey(long source, long sink)
    {
        Source = source;
        Sink = sink;
    }

    public long Source { get; }
    public long Sink { get; }

    protected bool Equals(TransferKey other)
    {
        return Source == other.Source && Sink == other.Sink;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((TransferKey)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Source, Sink);
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