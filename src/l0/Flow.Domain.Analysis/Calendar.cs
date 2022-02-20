namespace Flow.Domain.Analysis;

public class Calendar
{
    public Calendar(IEnumerable<Range> ranges, IEnumerable<string> dimensions, IEnumerable<Section> sections)
    {
        Ranges = ranges;
        Dimensions = dimensions;
        Sections = sections;
    }

    public IEnumerable<Range> Ranges { get; }

    public IEnumerable<string> Dimensions { get; }

    public IEnumerable<Section> Sections { get; }
}