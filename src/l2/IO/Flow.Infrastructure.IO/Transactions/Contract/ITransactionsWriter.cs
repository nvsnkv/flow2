using Flow.Domain.Transactions;

namespace Flow.Infrastructure.IO.Transactions.Contract;

public interface ITransactionsWriter : ISchemaSpecific
{
    Task WriteTransactions(StreamWriter writer, IEnumerable<Transaction> transactions, CancellationToken ct);

    Task WriteRecordedTransactions(StreamWriter writer, IEnumerable<RecordedTransaction> transactions, CancellationToken ct);
}
