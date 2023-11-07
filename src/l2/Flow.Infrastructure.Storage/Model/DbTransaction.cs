using Flow.Domain.Transactions;

namespace Flow.Infrastructure.Storage.Model;

// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
internal class DbTransaction : RecordedTransaction
{
    public DbTransaction(long key, DateTime timestamp, decimal amount, string currency, string? category, string title, string revision) : this(key, timestamp, amount, currency, category, title, DbAccount.Invalid, revision)
    {
    }

    public DbTransaction(DateTime timestamp, decimal amount, string currency, string? category, string title, DbAccount account, string revision) : this(0, timestamp, amount, currency, category, title, account, revision)
    {
    }

    public DbTransaction(long key, DateTime timestamp, decimal amount, string currency, string? category, string title, DbAccount account, string revision) : base(key, DateTime.SpecifyKind(timestamp, DateTimeKind.Utc), amount, currency, category, title, account, revision)
    {
    }

    public override AccountInfo Account => DbAccount;

    public virtual DbAccount DbAccount { get; set; } = null!;

    public virtual DbTransferKey? SourceOf { get; set; }

    public virtual DbTransferKey? SinkOf { get; set; }

}
