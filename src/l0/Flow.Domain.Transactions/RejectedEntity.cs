using System.Collections.Generic;

namespace Flow.Domain.Transactions;

public abstract class RejectedEntity<T>
{
    protected RejectedEntity(T entity, IReadOnlyList<string> reasons)
    {
        Entity = entity;
        Reasons = reasons;
    }

    public T Entity { get; }

    public IReadOnlyList<string> Reasons { get; }
}