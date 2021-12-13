using System.Collections.Generic;

namespace Flow.Domain.Transactions.Collections;

public class TransactionsWithDateRange<T> : ItemsWithDateRange<T> where T : Transaction
{
    public TransactionsWithDateRange(IEnumerable<T> items) : base(items, i => i.Timestamp)
    {
        
    }
}