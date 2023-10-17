using System.Linq.Expressions;

namespace Flow.Infrastructure.IO.Criteria;

internal class ParsingResult<TProp> : ParsingResult
{
    public ParsingResult(Expression<Func<TProp, bool>>? condition) : base(true, string.Empty)
    {
        Condition = condition;
    }

    public ParsingResult(string error) : base(false, error)
    {
    }

    public Expression<Func<TProp, bool>>? Condition { get; }
}
