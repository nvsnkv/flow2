using Flow.Application.Transactions.Contract;

namespace Flow.Infrastructure.IO.Import.Contract;

public interface IFileReader
{
    Task<IEnumerable<IncomingTransaction>> ReadFromFile(string path, CancellationToken ct);
}
