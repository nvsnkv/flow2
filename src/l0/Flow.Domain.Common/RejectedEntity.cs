namespace Flow.Domain.Common;

public abstract class RejectedEntity<T>
{
    protected RejectedEntity(T entity, IReadOnlyList<string> reasons)
    {
        Entity = entity;
        Reasons = reasons;
    }

    protected T Entity { get; }

    public IReadOnlyList<string> Reasons { get; }
}