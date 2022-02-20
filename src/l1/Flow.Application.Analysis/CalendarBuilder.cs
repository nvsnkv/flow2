using System.Collections.ObjectModel;
using Flow.Domain.Analysis;
using Flow.Domain.Analysis.Setup;
using Flow.Domain.Transactions;
using Range = Flow.Domain.Analysis.Range;

namespace Flow.Application.Analysis;

internal class CalendarBuilder
{
    private readonly List<SectionSetup> groups = new();
    private readonly List<Vector> dimensions = new();
    private readonly IAsyncEnumerable<RecordedTransaction> transactions;
    private readonly MonthlyRangesBuilder monthlyRangesBuilder;
    private Action<RejectedTransaction>? rejectionsHandler;
    private Vector? header;
    private Substitutor? substitutor;

    public CalendarBuilder(IAsyncEnumerable<RecordedTransaction> transactions, DateTime from, DateTime till)
    {
        this.transactions = transactions;
        monthlyRangesBuilder = new MonthlyRangesBuilder(from, till);
    }

    public CalendarBuilder WithHeader(Vector header)
    {
        this.header = header;
        return this;
    }

    public CalendarBuilder WithSubstitutor(Substitutor substitutor)
    {
        this.substitutor = substitutor;
        return this;
    }
    
    public CalendarBuilder WithAggregationGroup(SectionSetup group)
    {
        if (groups.Any(g => g.Name == group.Name)) { throw new ArgumentException("Group with this name already added!"); }
        groups.Add(group);
        
        AddMeasures(group);

        return this;
    }

    private void AddMeasures(SectionSetup? group)
    {
        while (group != null) {
            dimensions.AddRange(group.Rules.Select(r => r.Measure));
            group = group.Alternative;
        }
    }


    public CalendarBuilder WithRejectionsHandler(Action<RejectedTransaction> handler)
    {
        rejectionsHandler = handler;
        return this;
    }


    public async Task<Calendar> Build(CancellationToken ct)
    {
        var ranges = monthlyRangesBuilder.GetRanges().ToList().AsReadOnly();
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

        substitutor?.SortSubstitutions();

        var sections = rows
            .OrderBy(r => GetOrder(r.Key))
            .Select(r => 
                new Series(
                    r.Key,
                    r.Value.Select(b => b.Build())
                )
            );

        return new Calendar(ranges, header ?? Vector.Empty, sections);
    }

    private long GetOrder(Vector dimension)
    {
        if (!substitutor?.SubstitutionsSorted ?? false)
        {
            substitutor?.SortSubstitutions();
        }

        int idx = 0;

        while (idx < dimensions.Count)
        {
            var main= dimensions[idx];
            if (main == dimension)
            {
                return (long)idx << 32;
            }

            if (substitutor?.SubstitutionsMade.ContainsKey(main) ?? false)
            {
                var secIdx = substitutor.SubstitutionsMade[main].IndexOf(dimension);
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

    private Vector? GetMatchedVector(RecordedTransaction transaction, SectionSetup group)
    {
        var matchedDimensions = group.Rules.Where(r => r.Rule(transaction)).ToList();

        if (!matchedDimensions.Any())
        {
            if (group.Alternative == null)
            {
                if (rejectionsHandler != null)
                {
                    rejectionsHandler(new RejectedTransaction(transaction, $"Given transaction does not match to any aggregation rule!"));
                }

                return null;
            }

            return GetMatchedVector(transaction, group.Alternative);
        }

        if (matchedDimensions.Count > 1)
        {
            var matched = string.Join(", ", matchedDimensions.Select(r => $"[{string.Join(", ", r.Measure)}]"));
            if (rejectionsHandler != null)
            {
                rejectionsHandler(new RejectedTransaction(transaction, $"Given transaction matches with more than one aggregation rule ({matched}). Only first value will be used!"));
            }
        }

        var dimension = matchedDimensions.First().Measure;

        if (substitutor?.IsSubstitutionNeeded(dimension) ?? false)
        {
            dimension = substitutor.Substitute(dimension, transaction);
        }
        
        return dimension;
    }
}