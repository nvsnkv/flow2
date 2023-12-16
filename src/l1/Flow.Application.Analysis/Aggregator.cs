using System.Linq.Expressions;
using Flow.Application.Analysis.Contract;
using Flow.Application.Analysis.Contract.Setup;
using Flow.Application.ExchangeRates.Contract;
using Flow.Application.Transactions.Contract;
using Flow.Domain.Analysis;
using Flow.Domain.Patterns;
using Flow.Domain.Transactions;

namespace Flow.Application.Analysis;

internal class Aggregator : IAggregator
{
    private readonly IAccountant accountant;
    private readonly IExchangeRatesProvider ratesProvider;
    private readonly Substitutor substitutor;
    private readonly SeriesBuilderComparer comparer;

    public Aggregator(IAccountant accountant, IExchangeRatesProvider ratesProvider, Substitutor substitutor, SeriesBuilderComparer comparer)
    {
        this.accountant = accountant;
        this.ratesProvider = ratesProvider;
        this.substitutor = substitutor;
        this.comparer = comparer;
    }

    public async Task<(Calendar, IReadOnlyCollection<RejectedTransaction>)> GetCalendar(FlowConfig flowConfig, CalendarConfig calendarConfig, CancellationToken ct = default)
    {
        var (flow, rejectedItems) = await GetFlow(flowConfig, ct);
        var rejected = rejectedItems.ToList();

        var calendarBuilder = new CalendarBuilder(flow, flowConfig.From, flowConfig.Till)
            .WithRejectionsHandler(rejected.Add)
            .WithDimensions(calendarConfig.Dimensions)
            .WithSubstitutor(substitutor, comparer);

        calendarBuilder = calendarConfig.Series.Aggregate(calendarBuilder, (b, g) => b.WithSeries(g));

        var calendar = await calendarBuilder.Build(ct, calendarConfig.Depth);
        return (calendar, rejected.AsReadOnly());
    }

    public async Task<(IAsyncEnumerable<RecordedTransaction>, IEnumerable<RejectedTransaction>)> GetFlow(FlowConfig flowConfig, CancellationToken ct = default)
    {
        var (from, till, currency, criteria) = flowConfig;
        from = from.ToUniversalTime();
        till = till.ToUniversalTime();
        criteria ??= Constants<RecordedTransaction>.Truth;

        Expression<Func<RecordedTransaction, bool>> dateRange = t => from <= t.Timestamp && t.Timestamp < till;
        var transactions = await accountant.GetTransactions(dateRange, ct);
        var transfers = accountant.GetTransfers(dateRange, ct);

        var rejected = new List<RejectedTransaction>();
        var flow = new FlowBuilder(transactions)
            .WithTransfers(transfers)
            .InCurrency(currency, ratesProvider)
            .WithRejectionsHandler(rejected.Add)
            .Build(ct);

        return (flow.Where(criteria.Compile()).OrderByDescending(t => t.Timestamp), rejected);
    }
}