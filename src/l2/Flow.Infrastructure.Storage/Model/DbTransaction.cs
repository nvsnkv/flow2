using Flow.Domain.Transactions;

namespace Flow.Infrastructure.Storage.Model;

internal class DbTransaction : RecordedTransaction
{
    public DbTransaction(RecordedTransaction t) : this(t.Key, t)
    {
    }

    public DbTransaction(long key, DateTime timestamp, decimal amount, string currency, string? category, string title) : this(key, timestamp, amount, currency, category, title, DbAccount.Invalid)
    {
    }

    public DbTransaction(long key, DateTime timestamp, decimal amount, string currency, string? category, string title, AccountInfo account) : base(key, timestamp, amount, currency, category, title, account)
    {
    }

    public DbTransaction(long key, Transaction t) : base(key, t)
    {
    }

    public DbAccount DbAccount { get; set; } = null!;
}