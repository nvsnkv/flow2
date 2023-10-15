using Flow.Domain.Transactions;
using Flow.Infrastructure.Configuration.Contract;

namespace Flow.Infrastructure.Plugins.Transactions.Contract;

public interface ITransactionsReaderPlugin : IPlugin
{
    SupportedFormat SupportedFormat { get; }

    Task<IEnumerable<(Transaction, Overrides?)>> ReadTransactions(StreamReader reader, CancellationToken ct);

    Task<IEnumerable<RecordedTransaction>> ReadRecordedTransactions(StreamReader reader, CancellationToken ct);
}
