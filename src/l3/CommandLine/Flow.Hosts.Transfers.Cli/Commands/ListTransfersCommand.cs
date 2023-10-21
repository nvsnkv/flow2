using System.Linq.Expressions;
using Flow.Application.Transactions.Contract;
using Flow.Domain.Transactions;
using Flow.Domain.Transactions.Transfers;
using Flow.Hosts.Common.Commands;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Collections;
using Flow.Infrastructure.IO.Criteria.Contract;
using JetBrains.Annotations;

namespace Flow.Hosts.Transfers.Cli.Commands;

[UsedImplicitly]
internal class ListTransfersCommand : CommandBase
{
    private readonly IAccountant accountant;
    private readonly IWriters<Transfer> writers;
    private readonly ITransactionCriteriaParser parser;

    public ListTransfersCommand(IAccountant accountant, IWriters<Transfer> writers, ITransactionCriteriaParser parser, IFlowConfiguration config) : base(config)
    {
        this.accountant = accountant;
        this.writers = writers;
        this.parser = parser;
    }

    public async Task<int> Execute(ListTransfersArgs args, CancellationToken ct)
    {
        return await DoExecute(args, accountant.GetTransfers, ct);
    }

    public async Task<int> Execute(GuessTransfersArgs args, CancellationToken ct)
    {
        return await DoExecute(args, accountant.GuessTransfers, ct);
    }

    private async Task<int> DoExecute(ListArgsBase args, Func<Expression<Func<RecordedTransaction, bool>>, CancellationToken, IAsyncEnumerable<Transfer>> getTransfersFunc, CancellationToken ct)
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

        var transfers = await getTransfersFunc(criteria.Conditions!, ct).ToListAsync(cancellationToken: ct);

        var output = args.Output ?? (args.OpenEditor ? GetFallbackOutputPath(args.Format, "list", "transactions") : null);
        await using var streamWriter = CreateWriter(output);
        await writers.GetFor(args.Format).Write(streamWriter, transfers, ct);

        if (args.OpenEditor)
        {
            return await TryStartEditor(output, args.Format, false);
        }

        return 0;
    }
}
