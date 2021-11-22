using Flow.Domain.Transactions;

namespace Flow.Domain.Analysis;

public class Expense : FlowItem
{
    public Expense(RecordedTransaction transaction) : base(transaction)
    {
        if (transaction.Amount > 0) { throw new ArgumentException("Cannot create an expense from transaction with positive amount!", nameof(transaction)); }
    }
}