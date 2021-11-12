using System.Collections.Generic;
using System.Linq;

namespace Flow.Domain.Transactions;

public class RejectedTransaction :RejectedEntity<Transaction>
{
    public RejectedTransaction(Transaction transaction, params string[] reasons) : this(transaction, reasons.ToList().AsReadOnly())
    {
    }

    public RejectedTransaction(Transaction transaction, IReadOnlyList<string> reasons) : base(transaction, reasons)
    {
    }

    public Transaction Transaction => Entity;
}