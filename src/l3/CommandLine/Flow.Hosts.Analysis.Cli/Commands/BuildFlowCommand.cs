using Flow.Application.Analysis.Contract;
using Flow.Domain.Analysis.Setup;
using Flow.Domain.Common.Collections;
using Flow.Domain.Transactions;
using Flow.Hosts.Common.Commands;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Contract;

namespace Flow.Hosts.Analysis.Cli.Commands;


internal class BuildFlowCommand : CommandBase 
{
    private readonly IAggregator aggregator;
    private readonly ITransactionsWriter transactionWriter;
    private readonly IRejectionsWriter rejectionsWriter;
    private readonly ITransactionCriteriaParser parser;

    public BuildFlowCommand(IFlowConfiguration config, IAggregator aggregator, IRejectionsWriter rejectionsWriter, ITransactionsWriter transactionWriter, ITransactionCriteriaParser parser) : base(config)
    {
        this.aggregator = aggregator;
        this.rejectionsWriter = rejectionsWriter;
        this.transactionWriter = transactionWriter;
        this.parser = parser;
    }

    public async Task<int> Execute(BuildFlowArgs arg, CancellationToken ct)
    {
        var criteria = parser.ParseRecordedTransactionCriteria(arg.Criteria ?? Enumerable.Empty<string>());
        if (!criteria.Successful)
        {
            foreach (var error in criteria.Errors)
            {
                await Console.Error.WriteLineAsync(error);
                return 1;
            }
        }

        var config = new FlowConfig(arg.From.ToUniversalTime(), arg.Till.ToUniversalTime(), arg.Currency, criteria.Conditions);

        var (flow, rejections) = await aggregator.GetFlow(config, ct);

        var outputPath = arg.OutputPath ?? GetFallbackOutputPath(SupportedFormat.CSV, "flow", "list");
        await using (var writer = CreateWriter(outputPath))
        {
            await transactionWriter.WriteRecordedTransactions(writer, await flow.ToListAsync(ct), arg.Format, ct);
        }

        var rejectionsWithCount = new EnumerableWithCount<RejectedTransaction>(rejections);

        var rejectedPath = arg.RejectedPath ?? GetFallbackOutputPath(SupportedFormat.CSV, "flow", "rejected");
        await using (var rejWriter = CreateWriter(rejectedPath))
        {
            await rejectionsWriter.WriteRejections(rejWriter, rejectionsWithCount, arg.Format, ct);
        }

        if (rejectionsWithCount.Count > 0)
        {
            await TryStartEditor(rejectedPath, arg.Format, false);
        }

        await TryStartEditor(outputPath, arg.Format, false);
        return 0;
    }
}