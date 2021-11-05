namespace Flow.Domain.Patterns.Logical
{
    public static class LogicalPatternsExtensions
    {
        public static Pattern<T> Or<T>(this Pattern<T> pattern, Pattern<T> other)
        {
            return new OrPattern<T>(pattern, other);
        }

        public static Pattern<T> And<T>(this Pattern<T> pattern, Pattern<T> other)
        {
            return new AndPattern<T>(pattern, other);
        }
    }
}