using Flow.Domain.Transactions;
using Flow.Infrastructure.Configuration.Contract;

namespace Flow.Infrastructure.IO.Contract;
public interface ITransactionsReader
{
    IAsyncEnumerable<Transaction> ReadTransactions(StreamReader reader, SupportedFormat format, CancellationToken ct);

    IAsyncEnumerable<RecordedTransaction> ReadRecordedTransactions(StreamReader reader, SupportedFormat format, CancellationToken ct);
}