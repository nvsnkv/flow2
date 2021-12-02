using System.Linq.Expressions;
using Flow.Application.Analysis.Contract;
using Flow.Application.ExchangeRates.Contract;
using Flow.Application.Transactions.Contract;
using Flow.Domain.Analysis;
using Flow.Domain.Transactions;

namespace Flow.Application.Analysis;

internal class Aggregator : IAggregator
{
    private readonly IAccountant accountant;
    private readonly IExchangeRatesProvider ratesProvider;

    public Aggregator(IAccountant accountant, IExchangeRatesProvider ratesProvider)
    {
        this.accountant = accountant;
        this.ratesProvider = ratesProvider;
    }

    public async Task<(Calendar, IReadOnlyCollection<RejectedTransaction>)> GetCalendar(DateTime from, DateTime till, string currency, Vector header, IEnumerable<AggregationRule> dimensions, CancellationToken ct)
    {
        var (flow, rejectedItems) = await GetFlow(from, till, currency, ct);
        var rejected = rejectedItems.ToList();

        var calendarBuilder = new CalendarBuilder(flow, @from, till)
            .WithOffset(new MonthlyOffset())
            .WithRejectionsHandler(r => rejected.Add(r))
            .WithHeader(header);

        calendarBuilder = dimensions.Aggregate(calendarBuilder, (b, d) => b.WithAggregationRules(d));

        var calendar = await calendarBuilder.Build(ct);
        return (calendar, rejected.AsReadOnly());
    }

    public async Task<(IAsyncEnumerable<RecordedTransaction>, IEnumerable<RejectedTransaction>)> GetFlow(DateTime from, DateTime till, string currency, CancellationToken ct)
    {
        from = from.ToUniversalTime();
        till = till.ToUniversalTime();

        Expression<Func<RecordedTransaction, bool>> dateRange = t => from <= t.Timestamp && t.Timestamp < till;
        var transactions = await accountant.GetTransactions(dateRange, ct);
        var transfers = accountant.GetTransfers(dateRange, ct);

        var rejected = new List<RejectedTransaction>();
        var flow = new FlowBuilder(transactions)
            .WithTransfers(transfers)
            .InCurrency(currency, ratesProvider)
            .WithRejectionsHandler(r => rejected.Add(r))
            .Build(ct);

        return (flow.OrderByDescending(t => t.Timestamp), rejected);
    }
}