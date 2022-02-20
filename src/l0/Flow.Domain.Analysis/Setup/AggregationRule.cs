using Flow.Domain.Transactions;

namespace Flow.Domain.Analysis.Setup;

public class AggregationRule
{
    public AggregationRule(Vector dimensions, Func<RecordedTransaction, bool> rule)
    {
        Dimensions = dimensions;
        Rule = rule;
    }

    public Vector Dimensions { get; }

    public  Func<RecordedTransaction, bool> Rule { get; }
}