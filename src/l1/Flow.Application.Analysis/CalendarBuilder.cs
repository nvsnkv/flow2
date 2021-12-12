using System.Collections.ObjectModel;
using Flow.Domain.Analysis;
using Flow.Domain.Transactions;
using Range = Flow.Domain.Analysis.Range;

namespace Flow.Application.Analysis;

internal class CalendarBuilder
{
    private readonly List<AggregationGroup> groups = new();
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

    public CalendarBuilder WithAggregationGroup(AggregationGroup group)
    {
        if (groups.Any(g => g.Name == group.Name)) { throw new ArgumentException("Group with this name already added!"); }
        groups.Add(group);

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
        var rows = new Dictionary<Vector, List<AggregateBuilder>>();

        await foreach (var transaction in transactions.WithCancellation(ct))
        {
            var vectors = GetVectors(transaction);
            foreach (var vector in vectors)
            {
                if (!rows.ContainsKey(vector))
                {
                    var rowValues = new List<AggregateBuilder>();
                    while(rowValues.Count < ranges.Count)
                    {
                        rowValues.Add(new AggregateBuilder());
                    }
                    rows.Add(vector,rowValues);
                }

                var index = GetIndex(transaction, ranges);
                if (index >= 0)
                {
                    rows[vector][index].Append(transaction);
                }
            }
        }

        var values = new ReadOnlyDictionary<Vector, IReadOnlyList<Aggregate>>(
            rows.ToDictionary(
                r => r.Key, 
                r => (IReadOnlyList<Aggregate>)r.Value
                    .Select(b => b.Build())
                    .ToList()
                    .AsReadOnly()
                )
            );

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

    private IEnumerable<Vector> GetVectors(RecordedTransaction transaction)
    {
        return groups.Select(group => GetMatchedVector(transaction, group)).Where(v => v != null)!;
    }

    private Vector? GetMatchedVector(RecordedTransaction transaction, AggregationGroup group)
    {
        var matchedDimensions = group.Rules.Where(r => r.Rule(transaction)).ToList();

        if (!matchedDimensions.Any())
        {
            if (group.Subgroup == null)
            {
                if (rejectionsHandler != null)
                {
                    rejectionsHandler(new RejectedTransaction(transaction, $"Given transaction does not match to any aggregation rule!"));
                }

                return null;
            }

            return GetMatchedVector(transaction, group.Subgroup);
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