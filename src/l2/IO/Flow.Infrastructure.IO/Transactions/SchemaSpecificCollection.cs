using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Transactions.Contract;

namespace Flow.Infrastructure.IO.Transactions;

internal class SchemaSpecificCollection<T> : ISchemaSpecificCollection<T> where T : ISchemaSpecific
{
    private List<T> items;

    public SchemaSpecificCollection(IEnumerable<T> items)
    {
        this.items = items.ToList();
    }

    public T? FindFor(SupportedFormat format, SupportedDataSchema schema) => items.FirstOrDefault(i => i.Format == format && i.Schemas.Any(schema.Equals));

    public IEnumerable<T> GetKnown() => items;
}
