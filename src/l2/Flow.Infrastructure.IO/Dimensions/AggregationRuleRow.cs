using System.Linq.Expressions;
using Flow.Domain.Analysis;
using Flow.Domain.Transactions;

namespace Flow.Infrastructure.IO.Dimensions;

internal class AggregationRuleRow
{
    public AggregationRuleRow(Vector dimensions, Expression<Func<RecordedTransaction, bool>> rule)
    {
        Dimensions = dimensions;
        Rule = rule;
    }

    public Vector Dimensions { get; }
    
    public Expression<Func<RecordedTransaction, bool>> Rule { get; }
}