using Flow.Domain.Transactions;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Transactions.Contract;
using Flow.Infrastructure.Plugins.Transactions.Contract;

namespace Flow.Infrastructure.Plugins.Transactions.Loader;

internal class TransactionsReaderAdapter : ITransactionsReader
{
    private readonly ITransactionsReaderPlugin plugin;

    public TransactionsReaderAdapter(ITransactionsReaderPlugin plugin)
    {
        this.plugin = plugin;
    }

    public SupportedFormat Format => plugin.SupportedFormat;
    public SupportedDataSchema Schema => plugin.SupportedSchema;
    public Task<IEnumerable<(Transaction, Overrides?)>> ReadTransactions(StreamReader reader, CancellationToken ct)
    {
        return plugin.ReadTransactions(reader, ct);
    }

    public Task<IEnumerable<RecordedTransaction>> ReadRecordedTransactions(StreamReader reader, CancellationToken ct)
    {
        return plugin.ReadRecordedTransactions(reader, ct);
    }
}
