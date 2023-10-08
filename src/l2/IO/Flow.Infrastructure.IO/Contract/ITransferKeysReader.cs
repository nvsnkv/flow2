using Flow.Domain.Transactions.Transfers;
using Flow.Infrastructure.Configuration.Contract;

namespace Flow.Infrastructure.IO.Contract;

public interface ITransferKeysReader
{
    Task<IEnumerable<TransferKey>> ReadTransferKeys(StreamReader reader, SupportedFormat format, CancellationToken ct);
}