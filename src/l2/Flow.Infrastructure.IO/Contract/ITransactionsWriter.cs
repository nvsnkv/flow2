using Flow.Domain.Transactions;
using Flow.Infrastructure.Configuration.Contract;

namespace Flow.Infrastructure.IO.Contract;

public interface ITransactionsWriter
{
    Task WriteTransactions(StreamWriter writer, IAsyncEnumerable<Transaction> transactions, SupportedFormat format, CancellationToken ct);

    Task WriteRecordedTransactions(StreamWriter writer, IAsyncEnumerable<RecordedTransaction> transactions, SupportedFormat format, CancellationToken ct);
}