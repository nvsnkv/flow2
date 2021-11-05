using System;
using System.Linq.Expressions;

namespace Flow.Domain.Patterns
{
    public class PartialPattern<T, TProp> : Pattern<T>
    {
        private readonly Expression<Func<T, TProp>> selector;
        private readonly Pattern<TProp> pattern;

        public PartialPattern(Expression<Func<T, TProp>> selector, Pattern<TProp> pattern)
        {
            this.selector = selector;
            this.pattern = pattern;
        }

        protected internal override Expression<Func<T, bool>> GetExpression()
        {
            var param = Expression.Parameter(typeof(T));
            return Expression.Lambda<Func<T, bool>>(Expression.Invoke(pattern.GetExpression(), Expression.Invoke(selector, param)), param);
        }

        public static implicit operator PartialPattern<T, TProp>((Expression<Func<T, TProp>>, Pattern<TProp>) tuple)
        {
            return new PartialPattern<T, TProp>(tuple.Item1, tuple.Item2);
        }
    }
}