using System.Linq.Expressions;
using Flow.Application.Transactions.Contract;
using Flow.Domain.Transactions;
using Flow.Domain.Transactions.Transfers;
using Flow.Hosts.Common.Commands;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Contract;
using JetBrains.Annotations;

namespace Flow.Hosts.Transfers.Cli.Commands;

[UsedImplicitly]
internal class ListTransfersCommand : CommandBase
{
    private readonly IAccountant accountant;
    private readonly ITransfersWriter writer;
    private readonly ITransactionCriteriaParser parser;

    public ListTransfersCommand(IAccountant accountant, ITransfersWriter writer, ITransactionCriteriaParser parser, IFlowConfiguration config) : base(config)
    {
        this.accountant = accountant;
        this.writer = writer;
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

        var transfers = getTransfersFunc(criteria.Conditions!, ct);

        var output = args.Output ?? (args.OpenEditor ? GetFallbackOutputPath(args.Format, "list", "transactions") : null);
        await using var streamWriter = CreateWriter(output);
        await writer.WriteTransfers(streamWriter, transfers, args.Format, ct);

        if (args.OpenEditor)
        {
            return await TryStartEditor(output, args.Format, false);
        }

        return 0;
    }
}