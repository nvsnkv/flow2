using System.Text;
using Flow.Application.Transactions.Contract;
using Flow.Hosts.Common.Commands;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Contract;
using JetBrains.Annotations;

namespace Flow.Hosts.Accountant.Cli.Commands;

[UsedImplicitly]
internal class ListTransactionsCommand : CommandBase
{
    private readonly IAccountant accountant;
    private readonly ITransactionsWriter writer;
    private readonly ITransactionCriteriaParser parser;
    public ListTransactionsCommand(IFlowConfiguration config, ITransactionsWriter writer, ITransactionCriteriaParser parser, IAccountant accountant) : base(config)
    {
        this.writer = writer;
        this.parser = parser;
        this.accountant = accountant;
    }

    public async Task<int> Execute(ListTransactionsArgs args, CancellationToken ct)
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
        var transactions = await accountant.GetTransactions(criteria.Conditions, ct);

        var output = args.Output ?? (args.OpenEditor ? GetFallbackOutputPath(args.Format, "list", "transactions") : null);
        await using var streamWriter = CreateWriter(output);
        await writer.WriteRecordedTransactions(streamWriter, transactions, args.Format, ct);

        if (args.OpenEditor) 
        { 
            return await TryStartEditor(output, args.Format, false);
        }

        return 0;
    }
}