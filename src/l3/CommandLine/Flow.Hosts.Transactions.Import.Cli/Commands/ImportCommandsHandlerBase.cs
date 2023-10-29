using Flow.Domain.Transactions;
using Flow.Hosts.Common.Commands;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Collections;
using Flow.Infrastructure.IO.Contract;
using Flow.Infrastructure.IO.CSV.Contract;
using Flow.Infrastructure.IO.Import.Contract;

namespace Flow.Hosts.Transactions.Import.Cli.Commands;

internal class ImportCommandsHandlerBase : CommandBase
{
    protected readonly FolderBasedTransactionsImporter Importer;
    private readonly IWriters<RejectedTransaction> rejectionWriters;
    private readonly IWriters<RecordedTransaction> transactionWriters;

    protected ImportCommandsHandlerBase(IFlowConfiguration config, FolderBasedTransactionsImporter importer, IWriters<RejectedTransaction> rejectionWriters, IWriters<RecordedTransaction> transactionWriters) : base(config)
    {
        Importer = importer;
        this.rejectionWriters = rejectionWriters;
        this.transactionWriters = transactionWriters;
    }

    protected async Task WriteTransactions(string path, IEnumerable<RecordedTransaction> transactions, SupportedFormat format, CancellationToken ct)
    {
        await using var writer = new StreamWriter(File.OpenWrite(Path.Combine(Importer.Workspace, path)));
        await transactionWriters.GetFor(format).Write(writer, transactions, ct);
    }

    protected async Task WriteRejections(string path, IEnumerable<RejectedTransaction> rejected, CancellationToken ct)
    {
        var writer = rejectionWriters.GetFor(CSVIO.SupportedFormat);
        await using var streamWriter = new StreamWriter(File.OpenWrite(Path.Combine(Importer.Workspace, path)));
        await writer.Write(streamWriter, rejected, ct);
    }
}
