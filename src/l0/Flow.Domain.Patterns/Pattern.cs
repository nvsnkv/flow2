using System;
using System.Linq.Expressions;

namespace Flow.Domain.Patterns
{
    public abstract class Pattern<T>
    {
        public bool Match(T entity) 
        {
            var func = GetExpression().Compile();
            return func(entity);
        }

        protected internal abstract Expression<Func<T, bool>> GetExpression();

        public static explicit operator Expression<Func<T, bool>>(Pattern<T> pattern) 
        {
            return pattern.GetExpression();
        }
    }
}