using System.Runtime.CompilerServices;
using Flow.Application.ExchangeRates.Contract;
using Flow.Domain.ExchangeRates;
using Flow.Domain.Transactions;
using Flow.Domain.Transactions.Transfers;

namespace Flow.Application.Analysis;

internal class FlowBuilder
{
    private readonly IEnumerable<RecordedTransaction> transactions;
    private IAsyncEnumerable<Transfer>? transfers;

    private string? targetCurrency;
    private IExchangeRatesProvider? ratesProvider;

    private Action<RejectedTransaction>? rejectionsHandler;

    public FlowBuilder(IEnumerable<RecordedTransaction> transactions)
    {
        this.transactions = transactions;
    }

    public FlowBuilder WithTransfers(IAsyncEnumerable<Transfer> transfers)
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

    public FlowBuilder WithRejectionsHandler(Action<RejectedTransaction> handler)
    {
        rejectionsHandler = handler;
        return this;
    }


    public async IAsyncEnumerable<RecordedTransaction> Build([EnumeratorCancellation] CancellationToken ct)
    {
        var sources = await (transfers ?? AsyncEnumerable.Empty<Transfer>()).ToDictionaryAsync(s => s.Source.Key, ct);
        var sinks = sources.Values.Select(t => t.Sink.Key).ToHashSet();

        // meaningful transactions: transactions that changes amount of money within the system.
        var meaningfulTransactions = transactions
            .Where(t => { 
                if (sinks.Contains(t.Key))
                {
                    if (rejectionsHandler != null)
                    {
                        rejectionsHandler(new RejectedTransaction(t, "Given transaction is a transfer sink!"));
                    }

                    return false;
                }

                return true;
            })
            .Select(t => sources.ContainsKey(t.Key)
                ? new RecordedTransaction(t.Key, t.Timestamp, sources[t.Key].Fee, sources[t.Key].Currency, $"TRANSFER: {sources[t.Key].Comment}", $"{t.Category}: {t.Title}", t.Account)
                : t)
            .Select(t => !string.IsNullOrEmpty(t.Overrides?.Category) || !string.IsNullOrEmpty(t.Overrides?.Title)
                    ? new RecordedTransaction(t.Key, t.Timestamp, t.Amount, t.Currency, t.Overrides?.Category ?? t.Category, t.Overrides?.Title ?? t.Title, t.Account) { Overrides = t.Overrides }
                    : t)
            .Where(t =>
            {
                if (t.Amount == 0)
                {
                    if (rejectionsHandler != null)
                    {
                        rejectionsHandler(new RejectedTransaction(t, "Given transaction has zero amount!"));
                    }

                    return false;
                }

                return true;
            });

        foreach (var t in meaningfulTransactions)
        {
            var item = t;
            if (targetCurrency != null && t.Currency != targetCurrency)
            {
                item = await ConvertCurrency(t, ct);
            }

            if (item != null)
            {
                yield return item;
            }
        }
    }

    private async Task<RecordedTransaction?> ConvertCurrency(RecordedTransaction t, CancellationToken ct)
    {
        ExchangeRateRequest request = (t.Currency, targetCurrency!, t.Timestamp);
        var rate = await ratesProvider!.GetRate(request, ct);
        if (rate == null)
        {
            if (rejectionsHandler != null)
            {
                rejectionsHandler(new RejectedTransaction(t, "Unable to get exchange rate for given transaction!"));
            }

            return null;
        }

        return new RecordedTransaction(t.Key, t.Timestamp, t.Amount * rate.Rate, targetCurrency!, t.Category, t.Title,
            t.Account)
        { Overrides = t.Overrides };
    }
}
