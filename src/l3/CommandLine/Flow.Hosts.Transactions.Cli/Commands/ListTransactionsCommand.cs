using Flow.Application.Transactions.Contract;
using Flow.Domain.Transactions;
using Flow.Hosts.Common.Commands;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Collections;
using Flow.Infrastructure.IO.Criteria.Contract;
using JetBrains.Annotations;

namespace Flow.Hosts.Transactions.Cli.Commands;

[UsedImplicitly]
internal class ListTransactionsCommand : CommandBase
{
    private readonly IAccountant accountant;
    private readonly IWriters<RecordedTransaction> writers;
    private readonly ITransactionCriteriaParser parser;
    public ListTransactionsCommand(IFlowConfiguration config, IWriters<RecordedTransaction> writers, ITransactionCriteriaParser parser, IAccountant accountant) : base(config)
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

        var writer = writers.GetFor(args.Format);

        var transactions = args.DuplicatesOnly 
            ? (await accountant.GuessDuplicates(criteria.Conditions, args.DuplicatesDaysRange, ct)).SelectMany(d => d)
            : await accountant.GetTransactions(criteria.Conditions, ct);

        var output = args.Output ?? (args.OpenEditor ? GetFallbackOutputPath(args.Format, "list", "transactions") : null);
        await using var streamWriter = CreateWriter(output);
        await writer.Write(streamWriter, transactions, ct);

        if (args.OpenEditor) 
        { 
            return await TryStartEditor(output, args.Format, false);
        }

        return 0;
    }
}
