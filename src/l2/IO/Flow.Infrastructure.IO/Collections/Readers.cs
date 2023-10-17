using Flow.Infrastructure.IO.Contract;

namespace Flow.Infrastructure.IO.Collections;

internal sealed class Readers<T> : FormatSpecificCollection<IFormatSpecificReader<T>>, IReaders<T>
{
    public Readers(IEnumerable<IFormatSpecificReader<T>> items) : base(items) { }
}
