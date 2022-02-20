namespace Flow.Domain.Analysis.Setup;

public class AggregationSetup
{
    public AggregationSetup(IReadOnlyList<AggregationGroup> groups, Vector headers)
    {
        Groups = groups;
        Headers = headers;
    }

    public Vector Headers { get; }

    public IReadOnlyList<AggregationGroup> Groups { get; }
}