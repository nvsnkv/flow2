using System;
using System.Linq;
using System.Linq.Expressions;

namespace Flow.Domain.Patterns
{
    public class PatternBuilder<T>
    {
        private readonly Func<Expression, Expression, BinaryExpression> combineFunc;

        private Expression<Func<T, bool>> result;
        private ParameterExpression? param;

        public PatternBuilder(Expression<Func<T, bool>> seed, Func<Expression, Expression, BinaryExpression> combineFunc)
        {
            result = seed;
            this.combineFunc = combineFunc;
        }

        public PatternBuilder<T> With(Expression<Func<T, bool>> predicate)
        {
            param ??= predicate.Parameters.Single();

            var body = combineFunc(result.Body, Expression.Invoke(predicate, param));
            result = Expression.Lambda<Func<T, bool>>(body, param);
            return this;
        }

        public PatternBuilder<T> With<TProp>(Expression<Func<T, TProp>> selector, Expression<Func<TProp, bool>> predicate)
        {
            param ??= selector.Parameters.Single();

            var expr = Expression.Invoke(predicate, Expression.Invoke(selector, param));
            var body = combineFunc(result.Body, expr);
            result = Expression.Lambda<Func<T, bool>>(body, param);

            return this;
        }

        public Expression<Func<T, bool>> Build()
        {
            return result;
        }
    }
}