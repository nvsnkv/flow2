using System.Linq.Expressions;
using Flow.Application.Transactions.Infrastructure;
using Flow.Domain.Patterns;
using Flow.Domain.Transactions;
using FluentValidation;

namespace Flow.Application.Transactions;

internal class Accountant : IAccountant
{
    private readonly ITransactionsStorage storage;
    private readonly IValidator<Transaction> transactionValidator;
    
    public Accountant(ITransactionsStorage storage, IValidator<Transaction> transactionValidator)
    {
        this.storage = storage;
        this.transactionValidator = transactionValidator;
    }

    public async Task<IEnumerable<RecordedTransaction>> Get(Expression<Func<RecordedTransaction, bool>>? conditions,
        CancellationToken ct)
    {
        conditions ??= Constants<RecordedTransaction>.Truth;

        return await storage.Read(conditions, ct);
    }

    public async Task<IEnumerable<RejectedTransaction>> Create(IEnumerable<Transaction> transactions, CancellationToken ct)
    {
        var rejected = new List<RejectedTransaction>();
        var valid = transactions.Where(t => Validate(t, rejected));
        
        return (await storage.Create(valid, ct)).Concat(rejected);
    }

    public async Task<IEnumerable<RejectedTransaction>> Update(IEnumerable<RecordedTransaction> transactions, CancellationToken ct)
    {
        var rejected = new List<RejectedTransaction>();
        var valid = transactions.Where(t => Validate(t, rejected));

        return (await storage.Update(valid, ct)).Concat(rejected);
    }

    public async Task<int> Delete(Expression<Func<RecordedTransaction, bool>> conditions, CancellationToken ct)
    {
        return await storage.Delete(conditions, ct);
    }

    private bool Validate(Transaction t, ICollection<RejectedTransaction> rejected)
    {
        var result = transactionValidator.Validate(t);
        if (!result.IsValid)
        {
            rejected.Add(new RejectedTransaction(t, result.Errors.Select(e => e.ToString()).ToList().AsReadOnly()));
        }

        return result.IsValid;
    }
}