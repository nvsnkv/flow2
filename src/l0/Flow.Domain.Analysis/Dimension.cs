using Flow.Domain.Transactions;

namespace Flow.Domain.Analysis;

public class Dimension
{
    public Dimension(string name, IReadOnlyDictionary<string, Func<RecordedTransaction, bool>> rules)
    {
        Name = name;
        Rules = rules;
    }

    public string Name { get; }

    public IReadOnlyDictionary<string, Func<RecordedTransaction, bool>> Rules { get; }
}