using System.Linq.Expressions;

namespace Flow.Infrastructure.IO.Criteria.Contract;

public class CriteriaParserResult<T>
{
    public CriteriaParserResult(Expression<Func<T, bool>>? conditions)
    {
        Conditions = conditions;
        Errors = Enumerable.Empty<string>();
    }

    public CriteriaParserResult(IEnumerable<string> errors)
    {
        Errors = errors;
    }

    public Expression<Func<T, bool>>? Conditions { get; }

    public IEnumerable<string> Errors { get; }

    public bool Successful => Conditions != null;
}
