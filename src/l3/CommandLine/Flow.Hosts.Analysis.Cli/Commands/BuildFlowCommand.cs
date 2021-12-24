using Flow.Application.Analysis.Contract;
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

    public BuildFlowCommand(IFlowConfiguration config, IAggregator aggregator, IRejectionsWriter rejectionsWriter, ITransactionsWriter transactionWriter) : base(config)
    {
        this.aggregator = aggregator;
        this.rejectionsWriter = rejectionsWriter;
        this.transactionWriter = transactionWriter;
    }

    public async Task<int> Execute(BuildFlowArgs arg, CancellationToken ct)
    {

        var (flow, rejections) = await aggregator.GetFlow(arg.From.ToUniversalTime(), arg.Till.ToUniversalTime(), arg.Currency, ct);

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