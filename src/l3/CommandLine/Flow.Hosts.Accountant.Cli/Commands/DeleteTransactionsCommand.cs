using Flow.Application.Transactions;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Contract;

namespace Flow.Hosts.Accountant.Cli.Commands;

internal class DeleteTransactionsCommand : CommandBase
{
    private readonly ITransactionCriteriaParser parser;
    private readonly IAccountant accountant;

    public DeleteTransactionsCommand(IFlowConfiguration config, ITransactionCriteriaParser parser, IAccountant accountant) : base(config)
    {
        this.parser = parser;
        this.accountant = accountant;
    }

    public async Task<int> Execute(DeleteTransactionsArgs args, CancellationToken ct)
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

        await accountant.Delete(criteria.Conditions!, ct);

        return 0;
    }
}