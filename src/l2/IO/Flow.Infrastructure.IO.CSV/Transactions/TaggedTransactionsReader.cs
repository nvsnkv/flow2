using CsvHelper.Configuration;
using Flow.Domain.Analysis;

namespace Flow.Infrastructure.IO.CSV.Transactions;

internal class TaggedTransactionsReader : CsvReader<TaggedTransaction, TaggedTransactionRow>
{
    public TaggedTransactionsReader(CsvConfiguration config) : base(config, r => (TaggedTransaction)r)
    {
    }
}
