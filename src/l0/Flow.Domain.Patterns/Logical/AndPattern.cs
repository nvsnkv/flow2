using System;
using System.Linq.Expressions;

namespace Flow.Domain.Patterns.Logical
{
    public class AndPattern<T> : BinaryPattern<T>
    {
        public AndPattern(Pattern<T> left, Pattern<T> right) : base(left, right)
        {
        }

        protected internal override Expression<Func<T, bool>> GetExpression()
        {
            var l = Left.GetExpression();
            var r = Right.GetExpression();

            Expression expression = Expression.And(l.Body, r.Body);
            while (expression.CanReduce)
            {
                expression = expression.Reduce();
            }

            return Expression.Lambda<Func<T, bool>>(expression, l.Parameters);
        }
    }
}
