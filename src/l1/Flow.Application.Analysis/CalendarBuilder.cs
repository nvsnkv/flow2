using System.Collections.ObjectModel;
using Flow.Domain.Analysis;
using Flow.Domain.Transactions;
using Range = Flow.Domain.Analysis.Range;

namespace Flow.Application.Analysis;

internal class CalendarBuilder
{
    private readonly List<AggregationRule> rules = new();
    private readonly IAsyncEnumerable<RecordedTransaction> transactions;
    private readonly DateTime from;
    private readonly DateTime till;

    private Offset offset = new MonthlyOffset();
    private Action<RejectedTransaction>? rejectionsHandler;
    private Vector? header;

    public CalendarBuilder(IAsyncEnumerable<RecordedTransaction> transactions, DateTime from, DateTime till)
    {
        this.transactions = transactions;
        this.till = till;
        this.from = from;
    }

    public CalendarBuilder WithHeader(Vector header)
    {
        this.header = header;
        return this;
    }

    public CalendarBuilder WithAggregationRules(AggregationRule aggregationRule)
    {
        if (rules.Any(d => d.Dimensions.Equals(aggregationRule.Dimensions))) { throw new ArgumentException("Dimension with the same name was already added!"); }
        rules.Add(aggregationRule);

        return this;
    }

    public CalendarBuilder WithOffset(Offset offset)
    {
        this.offset = offset;
        return this;
    }

    public CalendarBuilder WithRejectionsHandler(Action<RejectedTransaction> handler)
    {
        rejectionsHandler = handler;
        return this;
    }


    public async Task<Calendar> Build(CancellationToken ct)
    {
        var ranges = GetRanges().ToList().AsReadOnly();
        var rows = new Dictionary<Vector, List<decimal?>>();

        await foreach (var transaction in transactions.WithCancellation(ct))
        {
            var vector = GetVector(transaction);
            if (vector != null)
            {
                if (!rows.ContainsKey(vector))
                {
                    rows.Add(vector, new List<decimal?>(Enumerable.Repeat((decimal?)null, ranges.Count)));
                }

                var index = GetIndex(transaction, ranges);
                if (index >= 0)
                {
                    var amount = rows[vector][index] ?? 0;
                    rows[vector][index] = amount + transaction.Amount;
                }
            }
        }

        var values = new ReadOnlyDictionary<Vector, IReadOnlyList<decimal?>>(rows.ToDictionary(r => r.Key, r => (IReadOnlyList<decimal?>)r.Value.AsReadOnly()));
        return new Calendar(ranges, header ?? Vector.Empty, values);
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

        if (rejectionsHandler != null)
        {
            rejectionsHandler(new RejectedTransaction(transaction, "Given transaction does not belong to any date range!"));
        }

        return -1;
    }

    private Vector? GetVector(RecordedTransaction transaction)
    {
        var matchedDimensions = rules.Where(r => r.Rule(transaction)).ToList();

        if(!matchedDimensions.Any())
        {
            if (rejectionsHandler != null)
            {
                rejectionsHandler(new RejectedTransaction(transaction, $"Given transaction does not match to any aggregation rule!"));
            }
            return null;
        }

        if (matchedDimensions.Count > 1)
        {
            var matched = string.Join(", ", matchedDimensions.Select(r => $"[{string.Join(", ", r.Dimensions)}]"));
            if (rejectionsHandler != null)
            {
                rejectionsHandler(new RejectedTransaction(transaction, $"Given transaction matches with more than one aggregation rule ({matched}). Only first value will be used!"));
            }
        }

        return matchedDimensions.First().Dimensions;

    }

    private IEnumerable<Range> GetRanges()
    {
        var end = from;

        do
        {
            var start = end;
            end = offset.ApplyTo(start);
            var range = new Range(start, till <= end ? till : end);
            range.Alias = offset.GetAliasFor(range);
            yield return range;
        } while (till > end);

    }
}