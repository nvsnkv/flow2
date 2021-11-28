using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Flow.Application.Analysis.Contract;
using Flow.Application.ExchangeRates.Contract;
using Flow.Application.Transactions.Contract;
using Flow.Domain.Analysis;
using Flow.Domain.Transactions;

namespace Flow.Application.Analysis;

internal class Analyst : IAnalyst
{
    private readonly IAccountant accountant;
    private readonly IExchangeRatesProvider ratesProvider;

    public Analyst(IAccountant accountant, IExchangeRatesProvider ratesProvider)
    {
        this.accountant = accountant;
        this.ratesProvider = ratesProvider;
    }

    public async IAsyncEnumerable<FlowItem> GetFlow(Expression<Func<RecordedTransaction, bool>> conditions, string? targetCurrency, [EnumeratorCancellation] CancellationToken ct)
    {
        var transactions = await accountant.GetTransactions(conditions, ct);
        var transfers = accountant.GetTransfers(conditions, ct);

        var builder = new FlowBuilder(transactions).WithTransfers(transfers);
        if (targetCurrency != null)
        {
            builder = builder.InCurrency(targetCurrency, ratesProvider);
        }

        var result = builder.Build(ct);
        await foreach (var item in result.WithCancellation(ct))
        {
            yield return item!;
        }
    }
}