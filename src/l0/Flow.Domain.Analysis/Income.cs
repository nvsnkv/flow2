using Flow.Domain.Transactions;

namespace Flow.Domain.Analysis;

public class Income : FlowItem
{
    public Income(RecordedTransaction transaction) : base(transaction)
    {
        if (transaction.Amount < 0) { throw new ArgumentException("Cannot create income from transaction with negative amount!", nameof(transaction)); }
    }
}