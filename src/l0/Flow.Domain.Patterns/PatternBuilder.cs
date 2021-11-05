using System;
using System.Linq;
using System.Linq.Expressions;
using Flow.Domain.Patterns.Logical;

namespace Flow.Domain.Patterns
{
    public class PatternBuilder<T>
    {
        private Expression<Func<T, bool>> result = Constants<T>.Truth;
        private ParameterExpression? param;
        
        public PatternBuilder<T> With(Expression<Func<T, bool>> predicate)
        {
            param ??= predicate.Parameters.Single();

            var body = Expression.AndAlso(result.Body, Expression.Invoke(predicate, param));
            result = Expression.Lambda<Func<T, bool>>(body, param);
            return this;
        }

        public PatternBuilder<T> With<TProp>(Expression<Func<T, TProp>> selector, Expression<Func<TProp, bool>> predicate)
        {
            param ??= selector.Parameters.Single();
            
            var expr = Expression.Invoke(predicate, Expression.Invoke(selector, param));
            var body = Expression.AndAlso(result.Body, expr);
            result = Expression.Lambda<Func<T, bool>>(body, param);

            return this;
        }

        public Expression<Func<T, bool>> Build()
        {
            return result;
        }
    }
}