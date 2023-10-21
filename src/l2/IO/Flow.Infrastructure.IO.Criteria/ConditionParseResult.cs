using System.Linq.Expressions;

namespace Flow.Infrastructure.IO.Criteria;

internal class ConditionParseResult<T, TProp> :ParsingResult<TProp>
{
    public ConditionParseResult(Expression<Func<T, TProp>>? selector, Expression<Func<TProp, bool>>? condition):base(condition)
    {
        Selector = selector;
    }

    public ConditionParseResult(string error) : base(error)
    {
    }

    public Expression<Func<T, TProp>>? Selector { get; }

    public override bool Successful => Selector != null && base.Successful;
}
