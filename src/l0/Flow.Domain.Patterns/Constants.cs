using System;
using System.Linq.Expressions;

namespace Flow.Domain.Patterns
{
    public static class Constants<T>
    {
        public static readonly Expression<Func<T, bool>> Truth = _ => true;

        public static readonly Expression<Func<T, bool>> Falsity = _ => false;
    }
}