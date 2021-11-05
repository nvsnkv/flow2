using System.Linq.Expressions;
using Flow.Domain.Transactions;

namespace Flow.Infrastructure.IO.Contract;

public class CriteriaParserResult<T> where T: Transaction
{
    public CriteriaParserResult(Expression<Func<T, bool>>? conditions, IEnumerable<string> errors)
    {
        Conditions = conditions;
        Errors = errors;
    }

    public Expression<Func<T, bool>>? Conditions { get; }

    public IEnumerable<string> Errors { get; }

    public bool Successful => Conditions != null;
}