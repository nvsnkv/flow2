using Flow.Application.Transactions.Contract;

namespace Flow.Application.Transactions.Infrastructure;

public interface ITransactionImporter
{
    Task<IImportContext> GetContext(CancellationToken ct);

    Task Save(IImportContext context, CancellationToken ct);

    Task Complete(IImportContext context, CancellationToken ct);
}
