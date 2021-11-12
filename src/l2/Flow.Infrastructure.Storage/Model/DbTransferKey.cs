using Flow.Domain.Transactions.Transfers;

namespace Flow.Infrastructure.Storage.Model;

internal class DbTransferKey : TransferKey
{
    public DbTransferKey(TransferKey t):this(t.Source, t.Sink)
    {
    }

    public DbTransferKey(long source, long sink) : base(source, sink)
    {
    }

    public virtual DbTransaction? SourceTransaction { get; set; }

    public virtual DbTransaction? SinkTransaction { get; set; }
}