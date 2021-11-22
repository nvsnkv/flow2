using Flow.Domain.Transactions;

namespace Flow.Domain.Analysis;
public abstract class FlowItem
{
    protected FlowItem(RecordedTransaction transaction)
    {
        Key = transaction.Key;
        Timestamp = transaction.Timestamp;
        Amount = transaction.Amount;
        Currency = transaction.Currency;
        Title = transaction.Overrides?.Title ?? transaction.Title;
        Category = transaction.Overrides?.Category ?? transaction.Category;
        Transaction = transaction;
    }

    public long Key { get; }

    public DateTime Timestamp { get; }

    public decimal Amount { get; }

    public string Currency { get; }

    public string Title { get; }

    public string Category { get; }

    public RecordedTransaction Transaction { get; }
}