namespace Flow.Domain.Analysis.Setup;

public class AggregationGroup
{
    public AggregationGroup(string name, IReadOnlyList<AggregationRule> rules, AggregationGroup? subgroup = null)
    {
        Name = name;
        Rules = rules;
        Subgroup = subgroup;
    }

    public string Name { get; }

    public IReadOnlyList<AggregationRule> Rules { get; }

    public AggregationGroup? Subgroup { get; }
}