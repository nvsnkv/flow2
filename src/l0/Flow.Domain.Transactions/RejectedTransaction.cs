using System.Collections.Generic;
using System.Linq;

namespace Flow.Domain.Transactions;

public class RejectedTransaction
{
    public RejectedTransaction(Transaction transaction, params string[] reasons) : this(transaction, reasons.ToList().AsReadOnly())
    {
    }

    public RejectedTransaction(Transaction transaction, IReadOnlyList<string> reasons)
    {
        Transaction = transaction;
        Reasons = reasons;
    }

    public Transaction Transaction { get; }

    public IReadOnlyList<string> Reasons { get; }
}