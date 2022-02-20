namespace Flow.Domain.Analysis;

public class Section
{
    public Section(IEnumerable<string> measure, IEnumerable<Aggregate> values)
    {
        Measure = measure;
        Values = values;
    }

    public IEnumerable<string> Measure { get; }

    public IEnumerable<Aggregate> Values { get; }
}