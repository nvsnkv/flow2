using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Contract;
using Flow.Infrastructure.IO.Transactions.Contract;

namespace Flow.Infrastructure.IO.Transactions;

internal class SchemaSpecificCollection<T> : ISchemaSpecificCollection<T> where T : ISchemaSpecific
{
    private List<T> items;

    public SchemaSpecificCollection(IEnumerable<T> items)
    {
        this.items = items.ToList();
    }

    public T? FindFor(SupportedFormat format, SupportedDataSchema schema) => items.FirstOrDefault(i => i.Format == format && schema.Equals(i.Schema));

    public IEnumerable<T> GetKnown() => items;
}
