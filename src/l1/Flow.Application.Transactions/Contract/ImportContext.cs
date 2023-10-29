using System.Linq.Expressions;
using Flow.Application.Transactions.Contract.Events;
using Flow.Domain.Transactions;

namespace Flow.Application.Transactions.Contract;

public sealed class ImportContext : IImportContext
{
    private readonly List<long> recordedTransactionKeys = new();
    private readonly INotifyTransactionRecorded notificator;
    private readonly TransactionRecordedEventHandler eventHandler;

    public ImportContext(INotifyTransactionRecorded notificator) : this(Enumerable.Empty<long>(), notificator)
    {
    }

    public ImportContext(IEnumerable<long> recordedTransactionKeys, INotifyTransactionRecorded notificator)
    {
        this.notificator = notificator;
        this.recordedTransactionKeys.AddRange(recordedTransactionKeys);

        eventHandler = NotificatorOnTransactionRecorded;
        this.notificator.TransactionRecorded += eventHandler;
    }

    public IEnumerable<long> RecordedTransactionKeys => recordedTransactionKeys.AsReadOnly();
    public int RecordedTransactionsCount => recordedTransactionKeys.Count;

    public Expression<Func<RecordedTransaction, bool>> Criteria => t => recordedTransactionKeys.Contains(t.Key);

    public ValueTask DisposeAsync()
    {
        notificator.TransactionRecorded -= eventHandler;
        return ValueTask.CompletedTask;
    }

    private void NotificatorOnTransactionRecorded(object sender, TransactionRecordedEventArgs args) => recordedTransactionKeys.Add(args.Key);
}
