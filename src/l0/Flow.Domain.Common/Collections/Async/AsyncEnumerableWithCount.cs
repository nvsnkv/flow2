namespace Flow.Domain.Common.Collections.Async;

public class AsyncEnumerableWithCount<T> : IAsyncEnumerable<T>
{
    private readonly IAsyncEnumerable<T> source;
    private int count;

    public bool Enumerated { get; internal set; }

    public int Count => Enumerated ? count : throw new InvalidOperationException("Collection was not enumerated!");
    public AsyncEnumerableWithCount(IAsyncEnumerable<T> source)
    {
        this.source = source.Select(i =>
        {
            count++;
            return i;
        });
    }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = new CancellationToken())
    {
        return new Enumerator<T>(source.GetAsyncEnumerator(cancellationToken), this);
    }

    private class Enumerator<TE> : IAsyncEnumerator<TE>
    {
        private readonly IAsyncEnumerator<TE> enumerator;
        private readonly AsyncEnumerableWithCount<TE> collection;

        public Enumerator(IAsyncEnumerator<TE> enumerator, AsyncEnumerableWithCount<TE> collection)
        {
            this.enumerator = enumerator;
            this.collection = collection;
        }

        public async ValueTask DisposeAsync()
        {
            await enumerator.DisposeAsync();
        }

        public async ValueTask<bool> MoveNextAsync()
        {
            var moved = await enumerator.MoveNextAsync();
            if (!moved)
            {
                collection.Enumerated = true;
            }

            return moved;
        }

        public TE Current => enumerator.Current;
    }
}