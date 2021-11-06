using Flow.Domain.Transactions;
using Microsoft.EntityFrameworkCore;

namespace Flow.Infrastructure.Storage;

internal partial class FlowDbContext : DbContext
{
    public FlowDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<RecordedTransaction> Transactions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccountInfo>(ab =>
        {
            ab.Property(a => a.Name).IsRequired();
            ab.Property(a => a.Bank).IsRequired();
            ab.HasKey(a => new { a.Name, a.Bank });
            ab.HasMany(typeof(RecordedTransaction))
                .WithOne()
                .HasForeignKey("account_name", "bank_name");
        });

        modelBuilder.Entity<RecordedTransaction>(tb =>
        {
            tb.Property(t => t.Key).ValueGeneratedOnAdd();
            tb.Property(t => t.Timestamp).IsRequired();
            tb.Property(t => t.Amount).IsRequired();
            tb.Property(t => t.Currency).IsRequired();
            tb.Property(t => t.Category);
            tb.Property(t => t.Title).IsRequired();
            tb.HasKey(t => t.Key);

            tb.Property(typeof(string), "account_name").IsRequired();
            tb.Property(typeof(string), "bank_name").IsRequired();

            tb.OwnsOne(t => t.Overrides, ob =>
            {
                ob.Property(o => o!.Title);
                ob.Property(o => o!.Category);
                ob.Property(o => o!.Comment);
            });
        });

        base.OnModelCreating(modelBuilder);
    }
}