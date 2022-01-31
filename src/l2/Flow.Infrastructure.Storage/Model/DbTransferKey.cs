using Flow.Domain.Transactions.Transfers;

namespace Flow.Infrastructure.Storage.Model;

internal class DbTransferKey : TransferKey
{
    public DbTransferKey(TransferKey t):this(t.SourceKey, t.SinkKey)
    {
    }

    public DbTransferKey(long sourceKey, long sinkKey) : base(sourceKey, sinkKey)
    {
    }

    public virtual DbTransaction? SourceTransaction { get; set; }

    public virtual DbTransaction? SinkTransaction { get; set; }
}