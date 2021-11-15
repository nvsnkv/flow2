using System.Collections;

namespace Flow.Domain.Common.Collections;

public class EnumerableWithCount<T> : IEnumerable<T>
{
    protected IEnumerable<T> Items;
    private int count;

    public bool Enumerated { get; private set; }

    public EnumerableWithCount(IEnumerable<T> items)
    {
        Items = items.Select(i =>
        {
            count++;
            return i;
        });
    }

    public int Count => Enumerated ? count : throw new InvalidOperationException("Collection was not enumerated!");

    public IEnumerator<T> GetEnumerator()
    {
        return new StatsEnumerator<T>(Items.GetEnumerator(), this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private class StatsEnumerator<TE> : IEnumerator<TE>
    {
        private readonly IEnumerator<TE> enumerator;
        private readonly EnumerableWithCount<TE> collection;

        public StatsEnumerator(IEnumerator<TE> enumerator, EnumerableWithCount<TE> collection)
        {
            this.enumerator = enumerator;
            this.collection = collection;
        }

        public bool MoveNext()
        {
            var moved = enumerator.MoveNext();
            if (!moved)
            {
                collection.Enumerated = true;
            }

            return moved;
        }

        public void Reset()
        {
            enumerator.Reset();
        }

        public TE Current => enumerator.Current;

        object? IEnumerator.Current => Current;

        public void Dispose()
        {
            enumerator.Dispose();
        }
    }
}