namespace Flow.Domain.Patterns.Logical
{
    public abstract class BinaryPattern<T> : Pattern<T>
    {
        protected readonly Pattern<T> Left;
        protected readonly Pattern<T> Right;

        protected BinaryPattern(Pattern<T> left, Pattern<T> right)
        {
            Left = left;
            Right = right;
        }
    }
}
