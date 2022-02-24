namespace Flow.Domain.Analysis.Setup;

public class CalendarConfig
{
    public CalendarConfig(IReadOnlyList<SeriesConfig> series, Vector dimensions)
    {
        Series = series;
        Dimensions = dimensions;
    }

    public Vector Dimensions { get; }

    public IReadOnlyList<SeriesConfig> Series { get; }
}