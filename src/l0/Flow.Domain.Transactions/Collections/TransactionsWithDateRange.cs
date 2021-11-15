using System;
using System.Collections.Generic;
using System.Linq;
using Flow.Domain.Common.Collections;

namespace Flow.Domain.Transactions.Collections;

public class TransactionsWithDateRange<T> : EnumerableWithCount<T> where T : Transaction
{
    private DateTime? min;
    private DateTime? max;

    public TransactionsWithDateRange(IEnumerable<T> items):base(items)
    {
        Items = Items.Select(i =>
        {
            if (min == null || min > i.Timestamp) min = i.Timestamp;
            if (max == null || max < i.Timestamp) max = i.Timestamp;

            return i;
        });
    }

    public DateTime? Min => Enumerated ? min : throw new InvalidOperationException("Collection was not enumerated!");

    public DateTime? Max => Enumerated ? max : throw new InvalidOperationException("Collection was not enumerated!");
}