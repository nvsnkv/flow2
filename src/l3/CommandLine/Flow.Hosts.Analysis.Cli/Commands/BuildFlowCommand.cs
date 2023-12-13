using Flow.Application.Analysis.Contract;
using Flow.Application.Analysis.Contract.Setup;
using Flow.Domain.Common.Collections;
using Flow.Domain.Transactions;
using Flow.Hosts.Common.Commands;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Collections;
using Flow.Infrastructure.IO.Criteria.Contract;
using Flow.Infrastructure.IO.CSV.Contract;

namespace Flow.Hosts.Analysis.Cli.Commands;


internal class BuildFlowCommand : CommandBase 
{
    private readonly IAggregator aggregator;
    private readonly IWriters<RecordedTransaction> transactionWriters;
    private readonly IWriters<RejectedTransaction> rejectionsWriters;
    private readonly ITransactionCriteriaParser parser;

    public BuildFlowCommand(IFlowConfiguration config, IAggregator aggregator, ITransactionCriteriaParser parser, IWriters<RejectedTransaction> rejectionsWriters, IWriters<RecordedTransaction> transactionWriters) : base(config)
    {
        this.aggregator = aggregator;
        this.parser = parser;
        this.rejectionsWriters = rejectionsWriters;
        this.transactionWriters = transactionWriters;
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

        var transactionWriter = transactionWriters.GetFor(args.Format);

        var config = new FlowConfig(args.From.ToUniversalTime(), args.Till.ToUniversalTime(), args.Currency, criteria.Conditions);

        var (flow, rejections) = await aggregator.GetFlow(config, ct);

        var outputPath = args.OutputPath ?? GetFallbackOutputPath(CSVIO.SupportedFormat, "flow", "list");


        await using (var writer = CreateWriter(outputPath))
        {
            await transactionWriter.Write(writer, await flow.ToListAsync(ct), ct);
        }

        var rejectionsWithCount = new EnumerableWithCount<RejectedTransaction>(rejections);

        var rejectedPath = args.RejectedPath ?? GetFallbackOutputPath(CSVIO.SupportedFormat, "flow", "rejected");
        await using (var rejWriter = CreateWriter(rejectedPath))
        {
            await rejectionsWriters.GetFor(args.Format).Write(rejWriter, rejectionsWithCount, ct);
        }

        if (rejectionsWithCount.Count > 0)
        {
            await TryStartEditor(rejectedPath, args.Format, false);
        }

        await TryStartEditor(outputPath, args.Format, false);
        return 0;
    }
}
