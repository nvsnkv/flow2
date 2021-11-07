using Flow.Domain.Transactions;
using Flow.Infrastructure.Configuration.Contract;

namespace Flow.Infrastructure.IO.Contract;
public interface ITransactionsReader
{
    Task<IEnumerable<Transaction>> ReadTransactions(StreamReader reader, SupportedFormat format, CancellationToken ct);

    Task<IEnumerable<RecordedTransaction>> ReadRecordedTransactions(StreamReader reader, SupportedFormat format, CancellationToken ct);
}

public interface ITransactionsWriter
{
    Task WriteTransactions(StreamWriter writer, IEnumerable<Transaction> transactions, SupportedFormat format, CancellationToken ct);

    Task WriteRecordedTransactions(StreamWriter writer, IEnumerable<RecordedTransaction> transactions, SupportedFormat format, CancellationToken ct);
}

public interface IRejectionsWriter
{
    Task WriteRejections(StreamWriter writer, IEnumerable<RejectedTransaction> rejections, SupportedFormat format, CancellationToken ct);
}