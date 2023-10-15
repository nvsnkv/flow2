using Flow.Domain.Transactions.Transfers;
using Flow.Infrastructure.Configuration.Contract;

namespace Flow.Infrastructure.IO.Transactions.Contract;

public interface ITransferKeysReader
{
    Task<IEnumerable<TransferKey>> ReadTransferKeys(StreamReader reader, OldSupportedFormat format, CancellationToken ct);
}