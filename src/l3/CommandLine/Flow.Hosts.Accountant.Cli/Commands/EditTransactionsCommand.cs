using Flow.Application.Transactions;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Contract;

namespace Flow.Hosts.Accountant.Cli.Commands;

internal class EditTransactionsCommand : CommandBase
{
    private readonly IAccountant accountant;
    private readonly ITransactionsReader reader;
    private readonly ITransactionsWriter writer;
    private readonly IRejectionsWriter rejectionsWriter;
    private readonly ITransactionCriteriaParser parser;

    public EditTransactionsCommand(IFlowConfiguration config, IRejectionsWriter rejectionsWriter, ITransactionsReader reader, IAccountant accountant, ITransactionCriteriaParser parser, ITransactionsWriter writer) : base(config)
    {
        this.rejectionsWriter = rejectionsWriter;
        this.reader = reader;
        this.accountant = accountant;
        this.parser = parser;
        this.writer = writer;
    }

    public async Task<int> Execute(UpdateTransactionsArgs args, CancellationToken ct)
    {
        using var streamReader = CreateReader(args.Input);

        var transactions = await reader.ReadTransactions(streamReader, args.Format, ct);
        var rejected = await accountant.Create(transactions, ct);

        var outputPath = args.Output ?? GetFallbackOutputPath(args.Format, "update", "rejected-transactions");
        await using var streamWriter = CreateWriter(outputPath);
        await rejectionsWriter.WriteRejections(streamWriter, rejected, args.Format, ct);

        return await TryStartEditor(outputPath, args.Format, false);
    }
    
    public async Task<int> Execute(EditTransactionsArgs args, CancellationToken ct)
    {
        var criteria = parser.ParseRecordedTransactionCriteria(args.Criteria ?? Enumerable.Empty<string>());
        if (!criteria.Successful)
        {
            foreach (var error in criteria.Errors)
            {
                await Console.Error.WriteLineAsync(error);
                return 1;
            }
        }

        var transactions = await accountant.Get(criteria.Conditions, ct);

        var interim = GetFallbackOutputPath(args.Format, "list", "transactions");
        await using (var streamWriter = CreateWriter(interim))
        {
            await writer.WriteRecordedTransactions(streamWriter, transactions, args.Format, ct);
        }

        var exitCode = await TryStartEditor(interim, args.Format, true);
        if (exitCode != 0)
        {
            await Console.Error.WriteLineAsync($"Failed to await external editor: error code {exitCode}");
            return exitCode;
        }

        using (var streamReader = CreateReader(interim))
        {
            transactions = await reader.ReadRecordedTransactions(streamReader, args.Format, ct);
        }

        var rejected = await accountant.Update(transactions, ct);
        var outputPath = args.Output ?? GetFallbackOutputPath(args.Format, "update", "rejected-transactions");
        
        await using (var streamWriter = CreateWriter(outputPath))
        {
            await rejectionsWriter.WriteRejections(streamWriter, rejected, args.Format, ct);
        }

        return await TryStartEditor(outputPath, args.Format, false);
    }
}