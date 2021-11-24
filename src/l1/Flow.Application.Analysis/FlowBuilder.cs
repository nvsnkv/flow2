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


    public IEnumerable<FlowItem> Build(CancellationToken ct)
    {
        var sources = (transfers ?? Enumerable.Empty<Transfer>()).ToDictionary(s => s.Source);
        var sinks = sources.Values.Select(t => t.Sink).ToHashSet();

        return transactions
            .Where(t => !sinks.Contains(t.Key))
            .Select(t => sources.ContainsKey(t.Key)
                ? new RecordedTransaction(t.Key, t.Timestamp, sources[t.Key].Fee, sources[t.Key].Currency, $"TRANSFER: {sources[t.Key].Comment}", $"{t.Category}: {t.Title}", t.Account)
                : t)
            .Where(t => t.Amount != 0)
            .Select(async t =>
            {
                if (targetCurrency != null && t.Currency != targetCurrency)
                {
                    var request = new ExchangeRateRequest(t.Currency, targetCurrency, t.Timestamp);
                    var rate = await ratesProvider!.GetRate(request, ct);
                    if (rate == null)
                    {
                        throw new InvalidOperationException("No exchange rate found!")
                        {
                            Data = { { "Request", request } }
                        };
                    }
                    return new RecordedTransaction(t.Key, t.Timestamp, t.Amount * rate.Rate, targetCurrency, t.Category, t.Title, t.Account) { Overrides = t.Overrides };
                }

                return t;
            })
            .Select(t => t.Await(ct))
            .Select(t => t.Amount > 0 ? (FlowItem)new Income(t) : new Expense(t));
    }
}
