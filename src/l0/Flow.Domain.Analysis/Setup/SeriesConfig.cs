using Flow.Domain.Transactions;

namespace Flow.Domain.Analysis.Setup;

public class SeriesConfig
{
    private static readonly IReadOnlyList<SeriesConfig> EmptySubSeriesList = new List<SeriesConfig>().AsReadOnly();

    public SeriesConfig(Vector measurement, IReadOnlyList<Func<RecordedTransaction, bool>> rules, IReadOnlyList<SeriesConfig>? subSeries = null)
    {
        Measurement = measurement;
        Rules = rules;

        SubSeries = subSeries ?? EmptySubSeriesList;
    }

    public Vector Measurement { get; }

    public IReadOnlyList<Func<RecordedTransaction, bool>> Rules { get; }

    public IReadOnlyList<SeriesConfig> SubSeries { get; }

    protected bool Equals(SeriesConfig other)
    {
        return Measurement.Equals(other.Measurement);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((SeriesConfig)obj);
    }

    public override int GetHashCode()
    {
        return Measurement.GetHashCode();
    }

    public static bool operator ==(SeriesConfig? left, SeriesConfig? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(SeriesConfig? left, SeriesConfig? right)
    {
        return !Equals(left, right);
    }
}