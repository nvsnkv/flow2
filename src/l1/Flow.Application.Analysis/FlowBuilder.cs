using System.Runtime.CompilerServices;
using Flow.Application.ExchangeRates.Contract;
using Flow.Domain.Analysis;
using Flow.Domain.Common;
using Flow.Domain.ExchangeRates;
using Flow.Domain.Transactions;
using Flow.Domain.Transactions.Transfers;

namespace Flow.Application.Analysis;
public class FlowBuilder
{
    private readonly IEnumerable<RecordedTransaction> transactions;
    private IEnumerable<Transfer>? transfers;

    private string? targetCurrency;
    private IExchangeRatesProvider? ratesProvider;

    public FlowBuilder(IEnumerable<RecordedTransaction> transactions)
    {
        this.transactions = transactions;
    }

    public FlowBuilder WithTransfers(IEnumerable<Transfer> transfers)
    {
        this.transfers = transfers;
        return this;
    }

    public FlowBuilder InCurrency(string targetCurrency, IExchangeRatesProvider ratesProvider)
    {
        this.targetCurrency = targetCurrency;
        this.ratesProvider = ratesProvider;

        return this;
    }


    public async IAsyncEnumerable<FlowItem> Build([EnumeratorCancellation] CancellationToken ct)
    {
        var sources = (transfers ?? Enumerable.Empty<Transfer>()).ToDictionary(s => s.Source);
        var sinks = sources.Values.Select(t => t.Sink).ToHashSet();

        // meaningful transactions: transactions that changes amount of money within the system.
        var meaningfulTransactions = transactions
            .Where(t => !sinks.Contains(t.Key))
            .Select(t => sources.ContainsKey(t.Key)
                ? new RecordedTransaction(t.Key, t.Timestamp, sources[t.Key].Fee, sources[t.Key].Currency, $"TRANSFER: {sources[t.Key].Comment}", $"{t.Category}: {t.Title}", t.Account)
                : t)
            .Where(t => t.Amount != 0);
        foreach (var t in meaningfulTransactions)
        {
            var item = t;
            if (targetCurrency != null && t.Currency != targetCurrency)
            {
                item = await ConvertCurrency(t, ct);
            }

            yield return item.Amount > 0 ? new Income(item) : new Expense(item);
        }
    }

    private async Task<RecordedTransaction> ConvertCurrency(RecordedTransaction t, CancellationToken ct)
    {
        ExchangeRateRequest request = (t.Currency, targetCurrency!, t.Timestamp);
        var rate = await ratesProvider!.GetRate(request, ct);
        if (rate == null)
        {
            throw new InvalidOperationException("No exchange rate found!")
            {
                Data = { { "Request", request } }
            };
        }

        return new RecordedTransaction(t.Key, t.Timestamp, t.Amount * rate.Rate, targetCurrency!, t.Category, t.Title,
            t.Account)
        { Overrides = t.Overrides };
    }
}
