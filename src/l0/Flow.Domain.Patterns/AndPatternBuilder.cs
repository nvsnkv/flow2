using System.Linq.Expressions;

namespace Flow.Domain.Patterns
{
    public class AndPatternBuilder<T> : PatternBuilder<T>
    {
        public AndPatternBuilder() : base(Constants<T>.Truth, Expression.AndAlso)
        {

        }
    }
}