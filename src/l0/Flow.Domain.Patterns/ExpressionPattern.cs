using System;
using System.Linq.Expressions;

namespace Flow.Domain.Patterns 
{
    public class ExpressionPattern<T> : Pattern<T>
    {
        private readonly Expression<Func<T, bool>> predicate;

        public ExpressionPattern(Expression<Func<T, bool>> predicate)
        {
            this.predicate = predicate;
        }

        protected internal override Expression<Func<T, bool>> GetExpression()
        {
            return predicate;
        }

        public static implicit operator ExpressionPattern<T>(Expression<Func<T, bool>> predicate)
        {
            return new ExpressionPattern<T>(predicate);
        }
    }
}