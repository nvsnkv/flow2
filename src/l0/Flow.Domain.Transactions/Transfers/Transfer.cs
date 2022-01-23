using System;

namespace Flow.Domain.Transactions.Transfers;

public class Transfer
{
    [Obsolete("Use ctor that accepts RecordedTransactions")]
    public Transfer(long source, long sink, decimal fee, string currency, DetectionAccuracy accuracyLevel)
    {
        if (source == sink) throw new ArgumentException("Source and sink must be different!", nameof(sink));

        Fee = fee;
        Currency = currency;
        AccuracyLevel = accuracyLevel;
        Source = new RecordedTransaction(source, default, default, string.Empty, null, string.Empty, new(string.Empty, string.Empty));
        Sink = new RecordedTransaction(sink, default, default, string.Empty, null, string.Empty, new(string.Empty, string.Empty));
    }

    public Transfer(RecordedTransaction source, RecordedTransaction sink, DetectionAccuracy accuracyLevel, decimal? fee = null, string? currency = null)
    {
        if (source.Key == sink.Key) throw new ArgumentException("Source and sink must be different!", nameof(sink));
        if (source.Currency != sink.Currency) throw new ArgumentException("Unable to create transfer object - currencies are different!", nameof(sink));
        if (source.Amount >= 0) throw new ArgumentException("Amount must be negative for source!", nameof(source));
        if (sink.Amount < 0) throw new ArgumentException("Amount must not be negative for sink!", nameof(sink));
        AccuracyLevel = accuracyLevel;

        Fee = fee ?? (source.Amount + sink.Amount);
        Currency = currency ?? source.Currency;
        Source = source;
        Sink = sink;
    }

    public RecordedTransaction Source { get; }

    public RecordedTransaction Sink { get; }

    public decimal Fee { get; }

    public string Currency { get; }

    public DetectionAccuracy AccuracyLevel { get; }

    public string? Comment { get; set; }

    protected bool Equals(Transfer other)
    {
        return Source.Equals(other.Source) && Sink.Equals(other.Sink);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Transfer)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Source.Key, Sink.Key);
    }
}