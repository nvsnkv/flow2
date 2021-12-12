namespace Flow.Domain.Analysis;

public class Calendar
{
    public Calendar(IReadOnlyList<Range> ranges, Vector dimensions, IReadOnlyDictionary<Vector, IReadOnlyList<Aggregate>> values)
    {
        Ranges = ranges;
        Dimensions = dimensions;
        Values = values;
    }

    public IReadOnlyList<Range> Ranges { get; }

    public Vector Dimensions { get; }

    public IReadOnlyDictionary<Vector, IReadOnlyList<Aggregate>> Values { get; }
}