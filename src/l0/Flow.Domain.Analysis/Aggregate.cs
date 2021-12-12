using Flow.Domain.Transactions;

namespace Flow.Domain.Analysis;

public class Aggregate
{
    public Aggregate(decimal? value, IEnumerable<RecordedTransaction> transactions)
    {
        Value = value;
        Transactions = transactions;
    }

    public decimal? Value { get; }

    public IEnumerable<RecordedTransaction> Transactions { get; }

}