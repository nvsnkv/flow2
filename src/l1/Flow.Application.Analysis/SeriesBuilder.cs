using Flow.Domain.Analysis;
using Flow.Domain.Analysis.Setup;
using Flow.Domain.Transactions;
using Range = Flow.Domain.Analysis.Range;

namespace Flow.Application.Analysis;

internal class SeriesBuilder
{
    private readonly IReadOnlyList<Range> ranges;
    private readonly List<AggregateBuilder> aggregates;
    private readonly List<SeriesBuilder> subSeries;

    private Substitutor? substitutor;
    private SeriesBuilderComparer? comparer;
    
    public SeriesBuilder(IReadOnlyList<Range> ranges, SeriesConfig config)
    {
        this.ranges = ranges;
        aggregates = new List<AggregateBuilder>(ranges.Select(_ => new AggregateBuilder()));

        Config = config;
        subSeries = new List<SeriesBuilder>(config.SubSeries.Select(c => new SeriesBuilder(ranges, c)));
    }

    public SeriesBuilder WithSubstitutor(Substitutor substitutor, SeriesBuilderComparer comparer)
    {
        this.substitutor = substitutor;
        this.comparer = comparer;

        for (var i = 0; i < subSeries.Count; i++)
        {
            subSeries[i] = subSeries[i].WithSubstitutor(substitutor, comparer);
        }
        
        return this;
    }

    public SeriesConfig Config { get; }

    public IEnumerable<SeriesBuilder> SubSeries => subSeries;

    public bool TryAppend(RecordedTransaction transaction, int? depth = null)
    {
        var index = GetIndex(transaction);
        return index != -1 && TryAppend(transaction, index, depth);
    }

    public IEnumerable<Series> Build(int dimensionsCount, int? depth = null)
    {
        if (depth == 0)
        {
            yield break;
        }

        var isSubstitutionNeeded = substitutor?.IsSubstitutionNeeded(Config.Measurement) ?? false;
        if (!isSubstitutionNeeded)
        {
            yield return new Series(Config.Measurement.PadRight(dimensionsCount), aggregates.Select(a => a.Build()));
        }

        if (substitutor?.IsSubstitutionNeeded(Config.Measurement) ?? false)
        {
            subSeries.Sort(comparer);
        }

        foreach (var builder in SubSeries)
        {
            foreach (var series in builder.Build(dimensionsCount, depth - (isSubstitutionNeeded ? 0 : 1)))
            {
                yield return series;
            }
        }
    }

    private bool TryAppend(RecordedTransaction transaction, int index, int? depth)
    {
        if (depth == 0)
        {
            return false;
        }

        if (Config.Rules.All(r => !r(transaction)))
        {
            return false;
        }

        if (substitutor?.IsSubstitutionNeeded(Config.Measurement) ?? false)
        {
            var measurement = substitutor.Substitute(Config.Measurement, transaction);
            var series = subSeries.FirstOrDefault(s => s.Config.Measurement == measurement);
            if (series == null)
            {
                series = new SeriesBuilder(ranges, new SeriesConfig(measurement, Config.Rules, Config.SubSeries));
                subSeries.Add(series);
            }

            series.TryAppend(transaction, index, depth);
        }
        else
        {
            aggregates[index].Append(transaction);
            var _ = SubSeries.Any(b => b.TryAppend(transaction, index, depth - 1));
        }
        
        

        return true;
    }

    private int GetIndex(RecordedTransaction transaction)
    {
        int index = 0;
        while (index < ranges.Count) 
        {
            if (ranges[index].Start <= transaction.Timestamp && transaction.Timestamp < ranges[index].End)
            {
                return index;
            }

            index++;
        }

        return -1;
    }

    public int GetDimensionsCount()
    {
        return SubSeries.Select(s => s.GetDimensionsCount()).Append(Config.Measurement.Count).Max();
    }
}