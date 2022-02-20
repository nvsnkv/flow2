namespace Flow.Domain.Analysis;

public class Series
{
    public Series(Vector measurement, IEnumerable<Aggregate> values)
    {
        Measurement = measurement;
        Values = values;
    }

    public Vector Measurement { get; }

    public IEnumerable<Aggregate> Values { get; }
}