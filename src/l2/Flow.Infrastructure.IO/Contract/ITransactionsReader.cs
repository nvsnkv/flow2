using Flow.Domain.Transactions;

namespace Flow.Infrastructure.IO.Contract;
public interface ITransactionsReader
{
    Task<IEnumerable<Transaction>> ReadTransactions(StreamReader reader, SupportedFormats format, CancellationToken ct);

    Task<IEnumerable<RecordedTransaction>> ReadRecordedTransactions(StreamReader reader, SupportedFormats format, CancellationToken ct);
}

public interface ITransactionsWriter
{
    Task WriteTransactions(StreamWriter writer, IEnumerable<Transaction> transactions, SupportedFormats format, CancellationToken ct);

    Task WriteRecordedTransactions(StreamWriter writer, IEnumerable<RecordedTransaction> transactions, SupportedFormats format, CancellationToken ct);
}

public interface IRejectionsWriter
{
    Task WriteRejections(StreamWriter writer, IEnumerable<RejectedTransaction> rejections, SupportedFormats formats, CancellationToken ct);
}