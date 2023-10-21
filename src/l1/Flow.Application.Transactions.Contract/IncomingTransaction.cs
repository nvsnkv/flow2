using Flow.Domain.Transactions;

namespace Flow.Application.Transactions.Contract;

public class IncomingTransaction
{
    public IncomingTransaction(Transaction t, Overrides? o)
    {
        Transaction = t;
        Overrides = o;
    }

    public Transaction Transaction { get; }

    public Overrides? Overrides { get; }

    public void Deconstruct(out Transaction transaction, out Overrides? overrides)
    {
        transaction = Transaction;
        overrides = Overrides;
    }
}
