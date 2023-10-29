using Flow.Application.Transactions.Contract;
using Flow.Infrastructure.IO.Import.Contract;

namespace Flow.Hosts.Transactions.Import.Cli.Commands;

internal class AbortCommandHandler
{
    private readonly FolderBasedTransactionsImporter importer;
    private readonly IAccountant accountant;

    public AbortCommandHandler(FolderBasedTransactionsImporter importer, IAccountant accountant)
    {
        this.importer = importer;
        this.accountant = accountant;
    }

    public async Task<int> Abort(AbortCommandArgs args, CancellationToken ct)
    {
        importer.Workspace = args.WorkingDirectory;
        var context = await importer.GetContext(ct);

        if (context.RecordedTransactionsCount > 0)
        {
            await accountant.DeleteTransactions(context.Criteria, ct);
        }

        await importer.Complete(context, ct);

        return 0;
    }
}
