using Flow.Application.Transactions.Contract;
using Flow.Domain.Transactions;
using Flow.Domain.Transactions.Transfers;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Collections;
using Flow.Infrastructure.IO.Contract;
using Flow.Infrastructure.IO.CSV.Contract;
using Flow.Infrastructure.IO.Import.Contract;

namespace Flow.Hosts.Transactions.Import.Cli.Commands;

internal class StartCommandHandler : ImportCommandsHandlerBase
{
    private readonly IAccountant accountant;
    private readonly IFileReader fileReader;
    private readonly IWriters<Transfer> transfersWriters;

    public StartCommandHandler(IFlowConfiguration config, IAccountant accountant, FolderBasedTransactionsImporter importer, IWriters<RejectedTransaction> rejectionWriters, IWriters<RecordedTransaction> transactionWriters, IWriters<Transfer> transfersWriters, IFileReader fileReader)
        : base(config, importer, rejectionWriters, transactionWriters)
    {
        this.accountant = accountant;
        this.transfersWriters = transfersWriters;
        this.fileReader = fileReader;
    }

    public async Task<int> Start(StartCommandArgs args, CancellationToken ct)
    {
        if (!Directory.Exists(args.WorkingDirectory))
        {
            await Console.Error.WriteLineAsync("Workspace folder does not exists!");
            return 1;
        }

        Importer.Workspace = args.WorkingDirectory;
        await using var context = await Importer.GetContext(ct);
        foreach (var file in Directory.EnumerateFiles(Importer.Workspace))
        {
            var data = await fileReader.ReadFromFile(file, ct);
            var rejected = await accountant.CreateTransactions(data, ct);

            await WriteRejections($"!rejected.{file}.csv", rejected, ct);

            await Importer.Save(context, ct);
        }

        var duplicates = await accountant.GuessDuplicates(context.Criteria, 3, ct);
        await WriteTransactions($"!duplicates.csv", duplicates.SelectMany(d => d), CSVIO.SupportedFormat, ct);

        var transfers = await accountant.GuessTransfers(context.Criteria, ct).ToListAsync(ct);
        await WriteTransfers("!transfers.csv", transfers, ct);

        return 0;
    }

    private async Task WriteTransfers(string path, IEnumerable<Transfer> transfers, CancellationToken ct)
    {
        await using var writer = new StreamWriter(File.OpenWrite(Path.Combine(Importer.Workspace, path)));
        await transfersWriters.GetFor(CSVIO.SupportedFormat).Write(writer, transfers, ct);
    }


}
