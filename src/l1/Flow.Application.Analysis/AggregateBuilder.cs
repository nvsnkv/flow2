using Flow.Domain.Analysis;
using Flow.Domain.Transactions;

namespace Flow.Application.Analysis;

internal class AggregateBuilder
{
    private readonly List<RecordedTransaction> transactions = new();
    private decimal? value;

    public AggregateBuilder Append(RecordedTransaction transaction)
    {
        transactions.Add(transaction);
        value = (value ?? 0) + transaction.Amount;

        return this;
    }

    public Aggregate Build()
    {
        return new Aggregate(value, transactions.AsReadOnly());
    }
}