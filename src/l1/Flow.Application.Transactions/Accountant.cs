using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Flow.Application.ExchangeRates.Contract;
using Flow.Application.Transactions.Contract;
using Flow.Application.Transactions.Infrastructure;
using Flow.Application.Transactions.Transfers;
using Flow.Domain.Patterns;
using Flow.Domain.Transactions;
using Flow.Domain.Transactions.Transfers;
using FluentValidation;

namespace Flow.Application.Transactions;

internal class Accountant : IAccountant
{
    private readonly ITransactionsStorage storage;
    private readonly IValidator<Transaction> transactionValidator;

    private readonly ITransferOverridesStorage transferKeyStorage;
    private readonly IList<ITransferDetector> transferDetectors;
    private readonly IValidator<TransferKey> transferKeyValidator;

    private readonly IExchangeRatesProvider ratesProvider;


    public Accountant(ITransactionsStorage storage, IValidator<Transaction> transactionValidator, IEnumerable<ITransferDetector> transferDetectors, ITransferOverridesStorage transferKeyStorage, IValidator<TransferKey> transferKeyValidator, IExchangeRatesProvider ratesProvider)
    {
        this.storage = storage;
        this.transactionValidator = transactionValidator;
        this.transferDetectors = transferDetectors.ToList();
        this.transferKeyStorage = transferKeyStorage;
        this.transferKeyValidator = transferKeyValidator;
        this.ratesProvider = ratesProvider;
    }

    public async Task<IEnumerable<RecordedTransaction>> GetTransactions(Expression<Func<RecordedTransaction, bool>>? conditions,
        CancellationToken ct)
    {
        conditions ??= Constants<RecordedTransaction>.Truth;

        return await storage.Read(conditions, ct);
    }

    public async Task<IEnumerable<RejectedTransaction>> CreateTransactions(IEnumerable<Transaction> transactions, CancellationToken ct)
    {
        var rejected = new List<RejectedTransaction>();
        var valid = transactions.Where(t => Validate(t, rejected));
        
        return (await storage.Create(valid, ct)).Concat(rejected);
    }

    public async Task<IEnumerable<RejectedTransaction>> UpdateTransactions(IEnumerable<RecordedTransaction> transactions, CancellationToken ct)
    {
        var rejected = new List<RejectedTransaction>();
        var valid = transactions.Where(t => Validate(t, rejected));

        return (await storage.Update(valid, ct)).Concat(rejected);
    }

    public async Task<int> DeleteTransactions(Expression<Func<RecordedTransaction, bool>> conditions, CancellationToken ct)
    {
        return await storage.Delete(conditions, ct);
    }

    public async IAsyncEnumerable<Transfer> GetTransfers(Expression<Func<RecordedTransaction, bool>> conditions, [EnumeratorCancellation] CancellationToken ct)
    {
        var transactions = await storage.Read(conditions, ct);
        
        var builder = transferDetectors.Aggregate(new TransfersBuilder(transactions.ToList()), (b, d) => b.With(d));
        builder.With(await OverridesBasedTransferDetector.Create(transferKeyStorage, ratesProvider, ct));

        await foreach (var t in builder.Build(ct))
        {
            yield return t;
        };
    }

    public async Task<IEnumerable<RejectedTransferKey>> EnforceTransfers(IEnumerable<TransferKey> keys, CancellationToken ct)
    {
        var rejections = new List<RejectedTransferKey>();
        return await transferKeyStorage.Enforce(keys.Where(k => Validate(k, rejections)), ct);
    }

    public async Task<IEnumerable<RejectedTransferKey>> AbandonTransfers(IEnumerable<TransferKey> keys, CancellationToken ct)
    {
        var rejections = new List<RejectedTransferKey>();
        return await transferKeyStorage.Abandon(keys.Where(k => Validate(k, rejections)), ct);
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

    private bool Validate(TransferKey t, ICollection<RejectedTransferKey> rejected)
    {
        var result = transferKeyValidator.Validate(t);
        if (!result.IsValid)
        {
            rejected.Add(new RejectedTransferKey(t, result.Errors.Select(e => e.ToString()).ToList().AsReadOnly()));
        }

        return result.IsValid;
    }
}