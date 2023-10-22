using Flow.Application.Transactions.Contract;

namespace Flow.Application.Transactions.Infrastructure;

public interface IImportContextsFinalizer
{
    Task Finalize(IImportContext context, CancellationToken ct);
}
