using Flow.Application.Transactions.Contract;
using Flow.Domain.Common.Collections;
using Flow.Domain.Transactions;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Collections;
using Flow.Infrastructure.IO.CSV.Contract;
using Flow.Infrastructure.IO.Import.Contract;

namespace Flow.Hosts.Transactions.Import.Cli.Commands;

internal class EditCommandHandler : ImportCommandsHandlerBase
{
    private readonly IAccountant accountant;
    private readonly IReaders<RecordedTransaction> recordedTransactionReaders;

    protected EditCommandHandler(IFlowConfiguration config, FolderBasedTransactionsImporter importer, IWriters<RejectedTransaction> rejectionWriters, IWriters<RecordedTransaction> transactionWriters, IAccountant accountant, IReaders<RecordedTransaction> recordedTransactionReaders)
        : base(config, importer, rejectionWriters, transactionWriters)
    {
        this.accountant = accountant;
        this.recordedTransactionReaders = recordedTransactionReaders;
    }

    public async Task<int> Edit(EditCommandArgs args, CancellationToken ct)
    {
        var interimPath = GetFallbackOutputPath(args.Format, "import-edit", "interim");
        if (string.IsNullOrEmpty(interimPath))
        {
            return 1;
        }

        Importer.Workspace = args.WorkingDirectory;
        var context = await Importer.GetContext(ct);

        await WriteTransactions(interimPath, await accountant.GetTransactions(context.Criteria, ct), args.Format, ct);

        var result = await TryStartEditor(interimPath, args.Format, true);
        if (result != 0)
        {
            return result;
        }

        using var streamReader = new StreamReader(File.OpenRead(interimPath));
        var updated = await recordedTransactionReaders.GetFor(args.Format).Read(streamReader, ct);

        var rejected = new EnumerableWithCount<RejectedTransaction>(await accountant.UpdateTransactions(updated, ct));
        var errsPath = args.ErrorsOutput ?? GetFallbackOutputPath(CSVIO.SupportedFormat, "import-edit", "rejected");

        if (errsPath != null)
        {
            await WriteRejections(errsPath, rejected, ct);
            if (rejected.Count > 0)
            {
                return await TryStartEditor(errsPath, args.Format, false);
            }

            return 0;
        }

        await Console.Error.WriteLineAsync("Failed to update several transactions! Please check stored transactions!");
        return 2;
    }
}
