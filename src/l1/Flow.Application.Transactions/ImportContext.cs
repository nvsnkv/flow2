using System.Linq.Expressions;
using Flow.Application.Transactions.Contract;
using Flow.Application.Transactions.Contract.Events;
using Flow.Application.Transactions.Infrastructure;
using Flow.Domain.Transactions;

namespace Flow.Application.Transactions;

internal class ImportContext : IImportContext
{
    private readonly List<long> recordedTransactionKeys = new();
    private readonly INotifyTransactionRecorded notificator;
    private readonly TransactionRecordedEventHandler eventHandler;

    private readonly IAccountant accountant;
    private readonly IImportContextsFinalizer finalizer;


    public ImportContext(INotifyTransactionRecorded notificator, IImportContextsFinalizer finalizer, IAccountant accountant) : this(Enumerable.Empty<long>(), notificator, finalizer, accountant)
    {
    }

    public ImportContext(IEnumerable<long> recordedTransactionKeys, INotifyTransactionRecorded notificator, IImportContextsFinalizer finalizer, IAccountant accountant)
    {
        this.notificator = notificator;
        this.finalizer = finalizer;
        this.accountant = accountant;
        this.recordedTransactionKeys.AddRange(recordedTransactionKeys);

        eventHandler = NotificatorOnTransactionRecorded;
        this.notificator.TransactionRecorded += eventHandler;
    }

    private void NotificatorOnTransactionRecorded(object sender, TransactionRecordedEventArgs args)
    {
        recordedTransactionKeys.Add(args.Key);
    }

    public ValueTask DisposeAsync()
    {
        notificator.TransactionRecorded -= eventHandler;
        return ValueTask.CompletedTask;
    }

    public int RecordedTransactionsCount => recordedTransactionKeys.Count;

    public Expression<Func<RecordedTransaction, bool>> ImportedTransactionsCriteria => t => recordedTransactionKeys.Contains(t.Key);

    public Task Complete(CancellationToken ct) => finalizer.Finalize(this, ct);

    public async Task Abort(CancellationToken ct)
    {
        await accountant.DeleteTransactions(ImportedTransactionsCriteria, ct);
        await finalizer.Finalize(this, ct);
    }
}
