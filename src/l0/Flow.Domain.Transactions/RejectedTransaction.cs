using System.Collections.Generic;
using System.Linq;
using Flow.Domain.Common;

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