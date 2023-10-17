using System.Linq.Expressions;

namespace Flow.Infrastructure.IO.Contract;

public interface ICriteriaParser<T>
{
    public Expression<Func<T, bool>> Parse(IEnumerable<string> input);
}
