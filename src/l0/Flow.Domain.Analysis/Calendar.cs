namespace Flow.Domain.Analysis;

public class Calendar
{
    public Calendar(IEnumerable<Range> ranges, Vector dimensions, IEnumerable<Series> series)
    {
        Ranges = ranges;
        Dimensions = dimensions;
        Series = series;
    }

    public IEnumerable<Range> Ranges { get; }

    public Vector Dimensions { get; }

    public IEnumerable<Series> Series { get; }
}