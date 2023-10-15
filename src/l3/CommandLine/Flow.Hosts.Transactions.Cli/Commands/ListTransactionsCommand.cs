using Flow.Application.Transactions.Contract;
using Flow.Hosts.Common.Commands;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Transactions.Contract;
using JetBrains.Annotations;

namespace Flow.Hosts.Transactions.Cli.Commands;

[UsedImplicitly]
internal class ListTransactionsCommand : CommandBase
{
    private readonly IAccountant accountant;
    private readonly ISchemaSpecificCollection<ITransactionsWriter> writers;
    private readonly ITransactionCriteriaParser parser;
    public ListTransactionsCommand(IFlowConfiguration config, ISchemaSpecificCollection<ITransactionsWriter> writers, ITransactionCriteriaParser parser, IAccountant accountant) : base(config)
    {
        this.writers = writers;
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

        var writer = writers.FindFor(args.Format);
        if (writer == null)
        {
            await Console.Error.WriteLineAsync($"No writer registered for format {args.Format}");
            return 2;
        }

        var transactions = args.DuplicatesOnly 
            ? (await accountant.GuessDuplicates(criteria.Conditions, args.DuplicatesDaysRange, ct)).SelectMany(d => d)
            : await accountant.GetTransactions(criteria.Conditions, ct);

        var output = args.Output ?? (args.OpenEditor ? GetFallbackOutputPath(args.Format, "list", "transactions") : null);
        await using var streamWriter = CreateWriter(output);
        await writer.WriteRecordedTransactions(streamWriter, transactions, ct);

        if (args.OpenEditor) 
        { 
            return await TryStartEditor(output, args.Format, false);
        }

        return 0;
    }
}
