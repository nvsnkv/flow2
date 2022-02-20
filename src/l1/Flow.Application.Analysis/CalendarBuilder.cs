using System.Collections.ObjectModel;
using System.Text;
using Flow.Domain.Analysis;
using Flow.Domain.Analysis.Setup;
using Flow.Domain.Transactions;
using Range = Flow.Domain.Analysis.Range;

namespace Flow.Application.Analysis;

internal class CalendarBuilder
{
    private readonly List<SeriesConfig> series = new();
    private readonly IAsyncEnumerable<RecordedTransaction> transactions;
    private readonly MonthlyRangesBuilder monthlyRangesBuilder;
    private Action<RejectedTransaction>? rejectionsHandler;
    private Vector? dimensions;
    private Substitutor? substitutor;

    public CalendarBuilder(IAsyncEnumerable<RecordedTransaction> transactions, DateTime from, DateTime till)
    {
        this.transactions = transactions;
        monthlyRangesBuilder = new MonthlyRangesBuilder(from, till);
    }

    public CalendarBuilder WithHeader(Vector header)
    {
        this.dimensions = header;
        return this;
    }

    public CalendarBuilder WithSubstitutor(Substitutor substitutor)
    {
        this.substitutor = substitutor;
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

    public async Task<Calendar> Build(CancellationToken ct)
    {
        var ranges = monthlyRangesBuilder.GetRanges().ToList().AsReadOnly();
        var rows = new Dictionary<SeriesConfig, List<AggregateBuilder>>();

        await foreach (var transaction in transactions.WithCancellation(ct))
        {
            var index = GetIndex(transaction, ranges);
            if (index < 0)
            {
                continue;
            }
            var configs = GetSeries(transaction, series, 0);
            foreach (var config in configs)
            {
                if (!rows.ContainsKey(config))
                {
                    var rowValues = new List<AggregateBuilder>();
                    while(rowValues.Count < ranges.Count)
                    {
                        rowValues.Add(new AggregateBuilder());
                    }
                    rows.Add(config,rowValues);
                }

                rows[config][index].Append(transaction);
            }
        }

        substitutor?.SortSubstitutions();

        var measurements = rows.Keys.Select(k => k.Measurement).ToList().AsReadOnly();

        var result = rows
            .OrderBy(r => GetOrder(r.Key.Measurement, measurements))
            .Select(r => new Series(r.Key.Measurement, r.Value.Select(b => b.Build())));

        return new Calendar(ranges, dimensions ?? Vector.Empty, result);
    }

    private long GetOrder(Vector measurement, IReadOnlyList<Vector> measurements)
    {
        if (!substitutor?.SubstitutionsSorted ?? false)
        {
            substitutor?.SortSubstitutions();
        }

        int idx = 0;

        while (idx < measurements.Count)
        {
            var main= measurements[idx];
            if (main == measurement)
            {
                return (long)idx << 32;
            }

            if (substitutor?.SubstitutionsMade.ContainsKey(main) ?? false)
            {
                var secIdx = substitutor.SubstitutionsMade[main].IndexOf(measurement);
                if (secIdx != -1)
                {
                    return ((long)idx << 32) + secIdx;
                }
            }

            idx++;
        }

        return -1;
    }

    private int GetIndex(Transaction transaction, IReadOnlyList<Range> ranges)
    {
        var result = 0;
        while (result < ranges.Count)
        {
            var range = ranges[result];
            if (range.Start <= transaction.Timestamp && transaction.Timestamp < range.End)
            {
                return result;
            }

            result++;
        }

        rejectionsHandler?.Invoke(new RejectedTransaction(transaction, "Given transaction does not belong to any date range!"));

        return -1;
    }

    private IEnumerable<SeriesConfig> GetSeries(RecordedTransaction transaction, IEnumerable<SeriesConfig> configs, int level)
    {
        var matched = configs.Where(c => c.Rules.Any(r => r(transaction))).ToList();
        if (matched.Count == 0)
        {
            rejectionsHandler?.Invoke(new RejectedTransaction(transaction, $"No suitable series found on level {level}"));
            yield break;
        }

        if (matched.Count > 1)
        {
            var message = matched
                .Select(m => m.ToString())
                .Aggregate(
                    new StringBuilder($"More than one suitable series found on level {level}:").AppendLine(),
                    (b, s) => b.AppendLine(s).AppendLine())
                .AppendLine("Only first match will be used!")
                .ToString();

            rejectionsHandler?.Invoke(new RejectedTransaction(transaction, message));
        }

        var result = matched.First();
        if (substitutor?.IsSubstitutionNeeded(result.Measurement) ?? false)
        {
            var replaced = substitutor.Substitute(result.Measurement, transaction);
            result = new SeriesConfig(replaced, result.Rules, result.SubSeries);
        }
        yield return result;

        if (result.SubSeries.Any())
        {
            foreach (var config in GetSeries(transaction, result.SubSeries, level + 1))
            {
                yield return config;
            }
        }
    }
}