using System;

namespace Flow.Domain.Transactions.Transfers;

public class Transfer : TransferKey
{
    public Transfer(long source, long sink, decimal fee, string currency):base(source, sink)
    {
        if (source == sink) throw new ArgumentException("Source and sink must be different!", nameof(sink));

        Fee = fee;
        Currency = currency;
    }

    public Transfer(RecordedTransaction source, RecordedTransaction sink) : base(source.Key, sink.Key)
    {
        if (source.Key == sink.Key) throw new ArgumentException("Source and sink must be different!", nameof(sink));
        if (source.Currency != sink.Currency) throw new ArgumentException("Unable to create transfer object - currencies are different!", nameof(sink));
        if (source.Amount >= 0) throw new ArgumentException("Amount must be negative for source!", nameof(source));
        if (sink.Amount < 0) throw new ArgumentException("Amount must not be negative for sink!", nameof(sink));
        
        Fee = source.Amount + sink.Amount;
        Currency = source.Currency;
    }

    public decimal Fee { get; }

    public string Currency { get; }

    public string? Comment { get; set; }
}