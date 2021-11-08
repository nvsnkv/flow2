using System;

namespace Flow.Domain.Transactions.Transfers;

public class Transfer
{
    public Transfer(long source, long sink, decimal fee, string currency)
    {
        if (source == sink) throw new ArgumentException("Source and sink must be different!", nameof(sink));

        Source = source;
        Sink = sink;
        Fee = fee;
        Currency = currency;
    }

    public Transfer(RecordedTransaction source, RecordedTransaction sink)
    {
        if (source.Key == sink.Key) throw new ArgumentException("Source and sink must be different!", nameof(sink));
        if (source.Currency != sink.Currency) throw new ArgumentException("Unable to create transfer object - currencies are different!", nameof(sink));
        if (source.Amount >= 0) throw new ArgumentException("Amount must be negative for source!", nameof(source));
        if (sink.Amount < 0) throw new ArgumentException("Amount must not be negative for sink!", nameof(sink));

        Source = source.Key;
        Sink = sink.Key;
        Fee = source.Amount + sink.Amount;
        Currency = source.Currency;
    }

    public long Source { get; }

    public long Sink { get; }

    public decimal Fee { get; }

    public string Currency { get; }
}