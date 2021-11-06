using System.Linq.Expressions;
using Flow.Application.Transactions.Infrastructure;
using Flow.Domain.Transactions;
using Microsoft.EntityFrameworkCore;

namespace Flow.Infrastructure.Storage;
internal class TransactionStorage : ITransactionsStorage
{
    private readonly IDbContextFactory<FlowDbContext> factory;

    public TransactionStorage(IDbContextFactory<FlowDbContext> factory)
    {
        this.factory = factory;
    }

    public async Task<IEnumerable<RejectedTransaction>> Create(IEnumerable<Transaction> transactions, CancellationToken ct)
    {
        await using var context = factory.CreateDbContext();
        foreach (var transaction in transactions)
        {
            await context.Transactions.AddAsync(new RecordedTransaction(0, transaction), ct);
        }

        await context.SaveChangesAsync(ct);

        return Enumerable.Empty<RejectedTransaction>();
    }

    public async Task<IEnumerable<RecordedTransaction>> Read(Expression<Func<Transaction, bool>> conditions, CancellationToken ct)
    {
        await using var context = factory.CreateDbContext();
        return await context.Transactions
            .Where(conditions)
            .Cast<RecordedTransaction>()
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<RejectedTransaction>> Update(IEnumerable<RecordedTransaction> transactions, CancellationToken ct)
    {
        await using var context = factory.CreateDbContext();
        var rejections = new List<RejectedTransaction>();
        foreach (var transaction in transactions)
        {
            var target = await context.Transactions.FirstOrDefaultAsync(t => t.Key == transaction.Key, ct);
            if (target is null)
            {
                rejections.Add(new RejectedTransaction(transaction,
                    $"Unable to identify transaction with key {transaction.Key}"));
            }
            else
            {
                context.Transactions.Remove(target);
                context.Transactions.Add(transaction);
            }
        }

        await context.SaveChangesAsync(ct);
        return rejections;
    }

    public async Task<int> Delete(Expression<Func<RecordedTransaction, bool>> conditions, CancellationToken ct)
    {
        await using var context = factory.CreateDbContext();
        var targets = await context.Transactions.Where(conditions).ToListAsync(ct);
        foreach (var target in targets)
        {
            context.Transactions.Remove(target);
        }

        return await context.SaveChangesAsync(ct);
    }
}