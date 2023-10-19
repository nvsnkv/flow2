using Flow.Infrastructure.IO.Contract;

namespace Flow.Infrastructure.IO.Collections;

internal sealed class Writers<T> : FormatSpecificCollection<IFormatSpecificWriter<T>, T>, IWriters<T>
{
    public Writers(IEnumerable<IFormatSpecificWriter<T>> items) : base(items) { }
}
