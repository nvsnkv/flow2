using Flow.Infrastructure.Configuration.Contract;

namespace Flow.Infrastructure.IO.Transactions.Contract;

public interface ISchemaSpecific
{
    public SupportedFormat Format { get; }

    public SupportedDataSchema Schema { get; }
}
