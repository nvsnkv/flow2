using Flow.Application.Analysis.Contract;
using Flow.Domain.Analysis.Setup;
using Flow.Domain.Common.Collections;
using Flow.Domain.Transactions;
using Flow.Hosts.Common.Commands;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Transactions.Contract;

namespace Flow.Hosts.Analysis.Cli.Commands;


internal class BuildFlowCommand : CommandBase 
{
    private readonly IAggregator aggregator;
    private readonly ISchemaSpecificCollection<ITransactionsWriter> transactionWriters;
    private readonly IRejectionsWriter rejectionsWriter;
    private readonly ITransactionCriteriaParser parser;

    public BuildFlowCommand(IFlowConfiguration config, IAggregator aggregator, IRejectionsWriter rejectionsWriter, ISchemaSpecificCollection<ITransactionsWriter> transactionWriters, ITransactionCriteriaParser parser) : base(config)
    {
        this.aggregator = aggregator;
        this.rejectionsWriter = rejectionsWriter;
        this.transactionWriters = transactionWriters;
        this.parser = parser;
    }

    public async Task<int> Execute(BuildFlowArgs args, CancellationToken ct)
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

        var transactionWriter = transactionWriters.FindFor(args.Format);
        if (transactionWriter == null)
        {
            await Console.Error.WriteLineAsync($"No writer registered for format {args.Format}");
            return 2;
        }

        var config = new FlowConfig(args.From.ToUniversalTime(), args.Till.ToUniversalTime(), args.Currency, criteria.Conditions);

        var (flow, rejections) = await aggregator.GetFlow(config, ct);

        var outputPath = args.OutputPath ?? GetFallbackOutputPath(SupportedFormat.CSV, "flow", "list");


        await using (var writer = CreateWriter(outputPath))
        {
            await transactionWriter.WriteRecordedTransactions(writer, await flow.ToListAsync(ct), ct);
        }

        var rejectionsWithCount = new EnumerableWithCount<RejectedTransaction>(rejections);

        var rejectedPath = args.RejectedPath ?? GetFallbackOutputPath(SupportedFormat.CSV, "flow", "rejected");
        await using (var rejWriter = CreateWriter(rejectedPath))
        {
            await rejectionsWriter.WriteRejections(rejWriter, rejectionsWithCount, args.Format, ct);
        }

        if (rejectionsWithCount.Count > 0)
        {
            await TryStartEditor(rejectedPath, args.Format, false);
        }

        await TryStartEditor(outputPath, args.Format, false);
        return 0;
    }
}
