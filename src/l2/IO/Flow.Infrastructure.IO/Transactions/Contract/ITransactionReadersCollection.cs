using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Contract;

namespace Flow.Infrastructure.IO.Transactions.Contract;

public interface ISchemaSpecificCollection<out T> where T:ISchemaSpecific
{
    T? FindFor(SupportedFormat format, SupportedDataSchema schema);

    IEnumerable<T> GetKnown();
}
