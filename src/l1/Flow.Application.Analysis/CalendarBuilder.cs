using System.Collections.ObjectModel;
using System.Dynamic;
using System.Text;
using Flow.Domain.Analysis;
using Flow.Domain.Transactions;
using Range = Flow.Domain.Analysis.Range;

namespace Flow.Application.Analysis;

internal class CalendarBuilder
{
    private readonly List<Dimension> dimensions = new();
    private readonly IAsyncEnumerable<RecordedTransaction> transactions;
    private readonly List<RejectedTransaction> rejections = new();
    private readonly DateTime from;
    private readonly DateTime till;

    private Offset offset = new MonthlyOffset();
    
    public CalendarBuilder(IAsyncEnumerable<RecordedTransaction> transactions, DateTime from, DateTime till)
    {
        this.transactions = transactions;
        this.till = till;
        this.from = from;
    }

    public CalendarBuilder WithDimension(Dimension dimension)
    {
        if (dimensions.Any(d => d.Name.Equals(dimension.Name))) { throw new ArgumentException("Dimension with the same name was already added!"); }
        dimensions.Add(dimension);

        return this;
    }

    public CalendarBuilder WithOffset(Offset offset)
    {
        this.offset = offset;
        return this;
    }


    public async Task<(Calendar, IEnumerable<RejectedTransaction>)> Build(CancellationToken ct)
    {
        var ranges = GetRanges().ToList().AsReadOnly();
        var rows = new Dictionary<Vector, List<decimal>>();

        await foreach (var transaction in transactions.WithCancellation(ct))
        {
            var vector = GetVector(transaction);
            if (vector != null)
            {
                if (!rows.ContainsKey(vector))
                {
                    rows.Add(vector, new List<decimal>(ranges.Count));
                }

                var index = GetIndex(transaction, ranges);
                if (index >= 0)
                {
                    rows[vector][index] += transaction.Amount; ;
                }
            }
        }

        var values = new ReadOnlyDictionary<Vector, IReadOnlyList<decimal>>(rows.ToDictionary(r => r.Key, r => (IReadOnlyList<decimal>)r.Value.AsReadOnly()));
        return (new Calendar(ranges, new Vector(dimensions.Select(d => d.Name)), values), rejections);
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
        }

        rejections.Add(new RejectedTransaction(transaction, "Given transaction does not belong to any date range!"));
        return -1;
    }

    private Vector? GetVector(RecordedTransaction transaction)
    {
        var values = new List<string>();
        foreach (var dimension in dimensions)
        {
            var dimensionValues = dimension.Rules.Where(r => r.Value(transaction)).Select(r => r.Key).ToList();
            if (!dimensionValues.Any())
            {
                rejections.Add(new RejectedTransaction(transaction, $"Given transaction does not match to any value from dimension {dimension.Name}"));
                return null;
            }

            if (dimensionValues.Count > 1)
            {
                var matched = string.Join(", ", dimensionValues);
                rejections.Add(new RejectedTransaction(transaction, $"Given transaction matches with more than one value from dimension {dimension.Name}: ({matched}). Only first value will be used!"));
            }

            values.Add(dimensionValues.First());
        }

        return new Vector(values);
    }

    private IEnumerable<Range> GetRanges()
    {
        var end = from;

        do
        {
            var start = end;
            end = offset.ApplyTo(start);
            yield return new Range(start, till <= end ? till : end);
        } while (till > end);

    }
}