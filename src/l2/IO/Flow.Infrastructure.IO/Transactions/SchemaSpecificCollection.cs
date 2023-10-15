using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Transactions.Contract;

namespace Flow.Infrastructure.IO.Transactions;

internal class SchemaSpecificCollection<T> : ISchemaSpecificCollection<T> where T : ISchemaSpecific
{
    private readonly List<T> items;

    public SchemaSpecificCollection(IEnumerable<T> items)
    {
        this.items = items.ToList();
    }

    public T? FindFor(SupportedFormat format) => items.FirstOrDefault(i => i.Format == format);

    public IEnumerable<T> GetKnown() => items;
}
