using Flow.Domain.ExchangeRates;
using Microsoft.EntityFrameworkCore;

namespace Flow.Infrastructure.Storage.Model;

internal class FlowDbContext : DbContext
{
    public FlowDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<DbAccount> Accounts { get; set; } = null!;

    public DbSet<DbTransaction> Transactions { get; set; } = null!;

    public DbSet<DbTransferKey> EnforcedTransfers { get; set; } = null!;

    public DbSet<ExchangeRate> ExchangeRates { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbAccount>(ab =>
        {
            ab.Property(a => a.Name).IsRequired();
            ab.Property(a => a.Bank).IsRequired();
            ab.HasKey(a => new { a.Name, a.Bank });
            ab.HasMany(a => a.Transactions).WithOne(t => t.DbAccount);
        });

        modelBuilder.Entity<DbTransferKey>(kb =>
        {
            kb.Property(k => k.SourceKey);
            kb.Property(k => k.SinkKey);
            kb.HasKey(k => new { Source = k.SourceKey, Sink = k.SinkKey });
        });

        modelBuilder.Entity<DbTransaction>(tb =>
        {
            tb.Property(t => t.Key).ValueGeneratedOnAdd();
            tb.Property(t => t.Timestamp).IsRequired();
            tb.Property(t => t.Amount).IsRequired();
            tb.Property(t => t.Currency).IsRequired();
            tb.Property(t => t.Category);
            tb.Property(t => t.Title).IsRequired();
            tb.HasKey(t => t.Key);

            tb.OwnsOne(t => t.Overrides, ob =>
            {
                ob.Property(o => o.Title);
                ob.Property(o => o.Category);
                ob.Property(o => o.Comment);
            });

            tb.HasOne(t => t.SourceOf!)
                .WithOne(k => k.SourceTransaction!)
                .HasForeignKey<DbTransferKey>(t => t.SourceKey);

            tb.HasOne(t => t.SinkOf!)
                .WithOne(k => k.SinkTransaction!)
                .HasForeignKey<DbTransferKey>(t => t.SinkKey);
        });

        modelBuilder.Entity<ExchangeRate>(rb =>
        {
            rb.Property(r => r.From);
            rb.Property(r => r.To);
            rb.Property(r => r.Date);
            rb.Property(r => r.Rate);

            rb.HasKey(r => new { r.From, r.To, r.Date });
        });

        base.OnModelCreating(modelBuilder);
    }
}