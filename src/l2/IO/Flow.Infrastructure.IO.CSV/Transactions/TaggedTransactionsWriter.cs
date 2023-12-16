using CsvHelper.Configuration;
using Flow.Domain.Analysis;

namespace Flow.Infrastructure.IO.CSV.Transactions;

internal class TaggedTransactionsWriter : CsvWriter<TaggedTransaction, TaggedTransactionRow, TaggedTransactionRowMap>
{
    public TaggedTransactionsWriter(CsvConfiguration config) : base(config, t => (TaggedTransactionRow)t)
    {
    }
}