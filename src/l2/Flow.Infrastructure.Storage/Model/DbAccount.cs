using Flow.Domain.Transactions;

namespace Flow.Infrastructure.Storage.Model;

// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
internal class DbAccount : AccountInfo
{
    public static DbAccount Invalid { get; } = new(string.Empty, string.Empty);

    public DbAccount(string name, string bank) : base(name, bank)
    {
    }

    public DbAccount(AccountInfo account) : base(account.Name, account.Bank)
    {
    }

    public virtual ICollection<DbTransaction> Transactions { get; set; } = null!;
}