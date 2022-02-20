using Flow.Domain.Transactions;

namespace Flow.Domain.Analysis.Setup;

public class SectionRule
{
    public SectionRule(Vector measure, Func<RecordedTransaction, bool> rule)
    {
        Measure = measure;
        Rule = rule;
    }

    public Vector Measure { get; }

    public  Func<RecordedTransaction, bool> Rule { get; }
}