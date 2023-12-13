using Flow.Application.Analysis.Contract.Setup;
using Flow.Domain.Analysis;
using Flow.Domain.Transactions;

namespace Flow.Application.Analysis;

internal class CalendarBuilder
{
    private readonly List<SeriesConfig> series = new();
    private readonly IAsyncEnumerable<RecordedTransaction> transactions;
    private readonly MonthlyRangesBuilder monthlyRangesBuilder;

    private Action<RejectedTransaction>? rejectionsHandler;
    private Vector? dimensions;
    private Substitutor? substitutor;
    private SeriesBuilderComparer? comparer;

    public CalendarBuilder(IAsyncEnumerable<RecordedTransaction> transactions, DateTime from, DateTime till)
    {
        this.transactions = transactions;
        monthlyRangesBuilder = new MonthlyRangesBuilder(from, till);
    }

    public CalendarBuilder WithDimensions(Vector dimensions)
    {
        this.dimensions = dimensions;
        return this;
    }

    public CalendarBuilder WithSubstitutor(Substitutor substitutor, SeriesBuilderComparer comparer)
    {
        this.substitutor = substitutor;
        this.comparer = comparer;
        return this;
    }
    
    public CalendarBuilder WithSeries(SeriesConfig config)
    {
        if (series.Any(s => s.Measurement == config.Measurement))
        {
            throw new ArgumentException("Series with these measurements were already added!", nameof(config));
        }

        series.Add(config);
        return this;
    }

    public CalendarBuilder WithRejectionsHandler(Action<RejectedTransaction> handler)
    {
        rejectionsHandler = handler;
        return this;
    }

    public async Task<Calendar> Build(CancellationToken ct, int? depth = null)
    {
        var ranges = monthlyRangesBuilder.GetRanges().ToList().AsReadOnly();
        var builders = series.Select(
            s =>
            substitutor != null
                ? new SeriesBuilder(ranges, s).WithSubstitutor(substitutor, comparer!)
                : new SeriesBuilder(ranges, s)
            ).ToList();

        await foreach (var transaction in transactions.WithCancellation(ct))
        {
            if (!builders.Any(b => b.TryAppend(transaction, depth)))
            {
                rejectionsHandler?.Invoke(new RejectedTransaction(transaction, "Given transaction does not belong to any group!"));
            }
        }

        var dimensionsCount = builders.Max(b => b.GetDimensionsCount());
        
        return new Calendar(ranges, dimensions?.PadRight(dimensionsCount) ?? Vector.Empty.PadRight(dimensionsCount), builders.SelectMany(b => b.Build(dimensionsCount, depth)));
    }
}