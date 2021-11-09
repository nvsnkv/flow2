using Flow.Domain.Transactions;

namespace Flow.Infrastructure.Storage.Model;

// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
internal class DbTransaction : RecordedTransaction
{
    public DbTransaction(long key, DateTime timestamp, decimal amount, string currency, string? category, string title) : this(key, timestamp, amount, currency, category, title, DbAccount.Invalid)
    {
    }

    public DbTransaction(DateTime timestamp, decimal amount, string currency, string? category, string title, DbAccount account) : this(0, timestamp, amount, currency, category, title, account)
    {
    }

    public DbTransaction(long key, DateTime timestamp, decimal amount, string currency, string? category, string title, DbAccount account) : base(key, timestamp, amount, currency, category, title, account)
    {
    }

    public override AccountInfo Account => DbAccount;

    public virtual DbAccount DbAccount { get; set; } = null!;

}