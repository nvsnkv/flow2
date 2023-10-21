using Flow.Infrastructure.IO.Contract;

namespace Flow.Infrastructure.IO.Collections;

internal class FormatSpecificCollection<T,TE> : ICollectionOfFormatSpecificItems<T, TE> where T : IFormatSpecific
{
    private readonly Dictionary<SupportedFormat, T> items;

    public FormatSpecificCollection(IEnumerable<T> items)
    {
        this.items = items.ToDictionary(i => i.Format);
    }

    public T GetFor(SupportedFormat format) => items.TryGetValue(format, out var res) ? res : throw new ArgumentException($"No format handler registered for {format} and {typeof(T).Name}!");

    public IEnumerable<SupportedFormat> GetKnownFormats() => items.Keys;
}
