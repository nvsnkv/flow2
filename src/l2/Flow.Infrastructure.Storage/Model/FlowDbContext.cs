﻿using Microsoft.EntityFrameworkCore;

namespace Flow.Infrastructure.Storage.Model;

internal class FlowDbContext : DbContext
{
    public FlowDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<DbAccount> Accounts { get; set; } = null!;

    public DbSet<DbTransaction> Transactions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbAccount>(ab =>
        {
            ab.Property(a => a.Name).IsRequired();
            ab.Property(a => a.Bank).IsRequired();
            ab.HasKey(a => new { a.Name, a.Bank });
            ab.HasMany(a => a.Transactions).WithOne(t => t.DbAccount);
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
                ob.Property(o => o!.Title);
                ob.Property(o => o!.Category);
                ob.Property(o => o!.Comment);
            });
        });

        base.OnModelCreating(modelBuilder);
    }
}