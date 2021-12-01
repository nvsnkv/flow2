using System.Linq.Expressions;
using Flow.Domain.Transactions;

namespace Flow.Infrastructure.IO.Dimensions;

internal class DimensionRule
{
    public DimensionRule(string name, string value, Expression<Func<RecordedTransaction, bool>> rule)
    {
        Name = name;
        Rule = rule;
        Value = value;
    }

    public string Name { get; }

    public string Value { get; }

    public Expression<Func<RecordedTransaction, bool>> Rule { get; }
}