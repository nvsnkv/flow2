using System;
using System.Linq.Expressions;

namespace Flow.Domain.Patterns.Logical
{
    public static class LogicalPatternsExtensions
    {
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> pattern, Expression<Func<T, bool>> other)
        {
            Expression expression = Expression.Or(pattern.Body, other.Body);

            while (expression.CanReduce)
            {
                expression = expression.Reduce();
            }

            return Expression.Lambda<Func<T, bool>>(expression, pattern.Parameters);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> pattern, Expression<Func<T, bool>> other)
        {
            Expression expression = Expression.And(pattern.Body, other.Body);
            
            while (expression.CanReduce)
            {
                expression = expression.Reduce();
            }

            return Expression.Lambda<Func<T, bool>>(expression, pattern.Parameters); 
        }
    }
}