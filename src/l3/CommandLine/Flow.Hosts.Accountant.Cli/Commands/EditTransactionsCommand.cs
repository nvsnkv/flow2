using Flow.Application.Transactions;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Contract;

namespace Flow.Hosts.Accountant.Cli.Commands;

internal class EditTransactionsCommand : CommandBase
{
    private readonly IAccountant accountant;
    private readonly ITransactionsReader reader;
    private readonly IRejectionsWriter writer;

    public EditTransactionsCommand(IFlowConfiguration config, IRejectionsWriter writer, ITransactionsReader reader, IAccountant accountant) : base(config)
    {
        this.writer = writer;
        this.reader = reader;
        this.accountant = accountant;
    }

    public async Task<int> Execute(UpdateTransactionsArgs args, CancellationToken ct)
    {
        using var streamReader = CreateReader(args.Input);

        var transactions = await reader.ReadTransactions(streamReader, args.Format, ct);
        var rejected = await accountant.Create(transactions, ct);

        var outputPath = args.Output ?? GetFallbackOutputPath(args.Format, "update", "rejected-transactions");
        await using var streamWriter = CreateWriter(outputPath);
        await writer.WriteRejections(streamWriter, rejected, args.Format, ct);

        return await TryStartEditor(outputPath, args.Format, false);
    }
}