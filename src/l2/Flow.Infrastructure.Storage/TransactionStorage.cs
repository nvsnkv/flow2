using System.Linq.Expressions;
using Flow.Application.Transactions.Infrastructure;
using Flow.Domain.Transactions;
using Flow.Infrastructure.Storage.Model;
using Microsoft.EntityFrameworkCore;

namespace Flow.Infrastructure.Storage;
internal class TransactionStorage : ITransactionsStorage
{
    private readonly IDbContextFactory<FlowDbContext> factory;

    public TransactionStorage(IDbContextFactory<FlowDbContext> factory)
    {
        this.factory = factory;
    }

    public async Task<IEnumerable<RejectedTransaction>> Create(IEnumerable<(Transaction, Overrides?)> transactions, CancellationToken ct)
    {
        await using var context = await factory.CreateDbContextAsync(ct);
        foreach (var t in transactions)
        {
            await AddTransaction(t, context, ct);
        }

        await context.SaveChangesAsync(ct);

        return Enumerable.Empty<RejectedTransaction>();
    }

    public async Task<IEnumerable<RecordedTransaction>> Read(Expression<Func<RecordedTransaction, bool>> conditions, CancellationToken ct)
    {
        await using var context = await factory.CreateDbContextAsync(ct);
        return await context.Transactions
            .Include(t => t.DbAccount)
            .Include(t => t.Overrides)
            .Where(conditions)
            .Cast<RecordedTransaction>()
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<RejectedTransaction>> Update(IEnumerable<RecordedTransaction> transactions, CancellationToken ct)
    {
        await using var context = await factory.CreateDbContextAsync(ct);
        var rejections = new List<RejectedTransaction>();
        foreach (var upd in transactions)
        {
            var target = await context.Transactions
                .Include(t => t.DbAccount)
                .Include(t => t.DbAccount.Transactions)
                .FirstOrDefaultAsync(t => t.Key == upd.Key, ct);

            if (target is null)
            {
                rejections.Add(new RejectedTransaction(upd, $"Unable to identify transaction with key {upd.Key}"));
            }
            else
            {
                if (((Transaction)upd) == ((Transaction)target))
                {
                    if (upd.Overrides != target.Overrides)
                    {
                        target.Overrides = upd.Overrides;
                    }
                }
                else
                {
                    target.DbAccount.Transactions.Remove(target);
                    if (target.DbAccount != upd.Account)
                    {
                        await AddTransaction((upd, upd.Overrides), context, ct);
                    }
                    else
                    {
                        target.DbAccount.Transactions.Add(new DbTransaction(upd.Key, upd.Timestamp, upd.Amount, upd.Currency, upd.Category, upd.Title, target.DbAccount) { Overrides = upd.Overrides });
                    }
                }
            }
        }

        await context.SaveChangesAsync(ct);
        return rejections;
    }

    public async Task<int> Delete(Expression<Func<RecordedTransaction, bool>> conditions, CancellationToken ct)
    {
        await using var context = await factory.CreateDbContextAsync(ct);
        var targets = await context.Transactions
            .Include(t => t.DbAccount)
            .Where(conditions).Cast<DbTransaction>().ToListAsync(ct);

        foreach (var target in targets)
        {
            target.DbAccount.Transactions.Remove(target);
        }

        return await context.SaveChangesAsync(ct);
    }

    private static async Task AddTransaction((Transaction, Overrides?) pair, FlowDbContext context, CancellationToken ct)
    {
        var (t, o) = pair;
        var account = await GetAccount(context, t.Account, ct);
        if (account == null)
        {
            account = new DbAccount(t.Account);
            await context.Accounts.AddAsync(account, ct);
            await context.SaveChangesAsync(ct);
            account = await GetAccount(context, t.Account, ct);
            if (account == null)
            {
                throw new InvalidOperationException("Failed to add an account to context!");
            }
        }

        var dbTransaction = new DbTransaction(t.Timestamp, t.Amount, t.Currency, t.Category, t.Title, account);
        if (o != null)
        {
            dbTransaction.Overrides = o;
        }

        account.Transactions.Add(dbTransaction);
    }

    private  static async Task<DbAccount?> GetAccount(FlowDbContext context, AccountInfo account, CancellationToken ct)
    {
        return await context.Accounts.Include(a => a.Transactions).SingleOrDefaultAsync(a => a.Name == account.Name && a.Bank == account.Bank, ct);
    }
}