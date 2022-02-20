namespace Flow.Domain.Analysis.Setup;

public class SectionSetup
{
    public SectionSetup(string name, IReadOnlyList<SectionRule> rules, SectionSetup? alternative = null)
    {
        Name = name;
        Rules = rules;
        Alternative = alternative;
    }

    public string Name { get; }

    public IReadOnlyList<SectionRule> Rules { get; }

    public SectionSetup? Alternative { get; }
}