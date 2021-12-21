using System;
using System.Collections.Generic;
using System.Linq;
using Flow.Domain.Common.Collections;

namespace Flow.Domain.Transactions.Collections;

public class ItemsWithDateRange<T> : EnumerableWithCount<T>
{
    private DateTime? min;
    private DateTime? max;

    public ItemsWithDateRange(IEnumerable<T> items, Func<T, DateTime> getTime) : base(items)
    {
        Items = Items.Select(i =>
        {
            var time = getTime(i);
            
            if (min == null || min > time) min = time;
            if (max == null || max < time) max = time;

            return i;
        });
    }
    

    public DateTime? Min => Enumerated ? min : throw new InvalidOperationException("Collection was not enumerated!");
    public DateTime? Max => Enumerated ? max : throw new InvalidOperationException("Collection was not enumerated!");
}