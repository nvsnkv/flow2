namespace Flow.Domain.Analysis.Setup;

public class AggregationSetup
{
    public AggregationSetup(IReadOnlyList<SectionSetup> sections, Vector dimensions)
    {
        Sections = sections;
        Dimensions = dimensions;
    }

    public Vector Dimensions { get; }

    public IReadOnlyList<SectionSetup> Sections { get; }
}