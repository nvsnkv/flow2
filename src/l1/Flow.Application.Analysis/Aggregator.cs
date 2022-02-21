using System.Linq.Expressions;
using Flow.Application.Analysis.Contract;
using Flow.Application.ExchangeRates.Contract;
using Flow.Application.Transactions.Contract;
using Flow.Domain.Analysis;
using Flow.Domain.Analysis.Setup;
using Flow.Domain.Transactions;

namespace Flow.Application.Analysis;

internal class Aggregator : IAggregator
{
    private readonly IAccountant accountant;
    private readonly IExchangeRatesProvider ratesProvider;
    private readonly Substitutor substitutor;

    public Aggregator(IAccountant accountant, IExchangeRatesProvider ratesProvider, Substitutor substitutor)
    {
        this.accountant = accountant;
        this.ratesProvider = ratesProvider;
        this.substitutor = substitutor;
    }

    public async Task<(Calendar, IReadOnlyCollection<RejectedTransaction>)> GetCalendar(DateTime from, DateTime till, string currency, CalendarConfig setup, CancellationToken ct)
    {
        var (flow, rejectedItems) = await GetFlow(from, till, currency, ct);
        var rejected = rejectedItems.ToList();

        var calendarBuilder = new CalendarBuilder(flow, from, till)
            .WithRejectionsHandler(r => rejected.Add(r))
            .WithDimensions(setup.Dimensions)
            .WithSubstitutor(substitutor);

        calendarBuilder = setup.Series.Aggregate(calendarBuilder, (b, g) => b.WithSeries(g));

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