using Flow.Domain.Transactions;

namespace Flow.Infrastructure.IO.Transactions.Contract;
public interface ITransactionsReader : ISchemaSpecific
{
    Task<IEnumerable<(Transaction, Overrides?)>> ReadTransactions(StreamReader reader, CancellationToken ct);

    Task<IEnumerable<RecordedTransaction>> ReadRecordedTransactions(StreamReader reader, CancellationToken ct);
}
