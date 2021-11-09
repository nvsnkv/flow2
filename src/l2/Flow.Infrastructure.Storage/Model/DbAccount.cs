using Flow.Domain.Transactions;

namespace Flow.Infrastructure.Storage.Model;

internal class DbAccount : AccountInfo
{
    public static AccountInfo Invalid { get; } = new DbAccount(string.Empty, string.Empty);

    public DbAccount(string name, string bank) : base(name, bank)
    {
    }

    public ICollection<DbTransaction> Transactions { get; set; } = null!;
}