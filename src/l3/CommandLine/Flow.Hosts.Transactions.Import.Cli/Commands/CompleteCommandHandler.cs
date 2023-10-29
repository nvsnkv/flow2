using Flow.Infrastructure.IO.Import.Contract;

namespace Flow.Hosts.Transactions.Import.Cli.Commands;

internal class CompleteCommandHandler
{
    private readonly FolderBasedTransactionsImporter importer;

    public CompleteCommandHandler(FolderBasedTransactionsImporter importer)
    {
        this.importer = importer;
    }

    public async Task<int> Complete(CompleteCommandArgs args, CancellationToken ct)
    {
        importer.Workspace = args.WorkingDirectory;
        var context = await importer.GetContext(ct);

        await importer.Complete(context, ct);
        return 0;
    }
}
