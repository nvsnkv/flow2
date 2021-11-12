using Flow.Domain.Transactions;
using Flow.Infrastructure.Configuration.Contract;

namespace Flow.Infrastructure.IO.Contract;
public interface ITransactionsReader
{
    Task<IEnumerable<Transaction>> ReadTransactions(StreamReader reader, SupportedFormat format, CancellationToken ct);

    Task<IEnumerable<RecordedTransaction>> ReadRecordedTransactions(StreamReader reader, SupportedFormat format, CancellationToken ct);
}