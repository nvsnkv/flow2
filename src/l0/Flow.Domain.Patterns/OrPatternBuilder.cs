using System.Linq.Expressions;

namespace Flow.Domain.Patterns
{
    public class OrPatternBuilder<T> : PatternBuilder<T>
    {
        public OrPatternBuilder() : base(Constants<T>.Falsity, Expression.OrElse)
        {

        }
    }
}