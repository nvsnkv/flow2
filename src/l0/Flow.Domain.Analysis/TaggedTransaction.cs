using Flow.Domain.Transactions;

namespace Flow.Domain.Analysis;

public class TaggedTransaction : RecordedTransaction
{
    public TaggedTransaction(RecordedTransaction t, IEnumerable<Tag> tags) : base(t.Key, t, t.Revision)
    {
        Overrides = t.Overrides;
        Tags = tags.ToList().AsReadOnly();
    }

    public IReadOnlyCollection<Tag> Tags { get; }
}
