using CsvHelper.Configuration;
using Flow.Domain.Transactions;

namespace Flow.Infrastructure.IO.CSV.Transactions;

internal class TransactionsReader : CsvReader<(Transaction, Overrides?),TransactionRow>
{
    public TransactionsReader(CsvConfiguration config) : base(config, (TransactionRow r) =>
    {
        var (transaction, overrides) = r;
        return (transaction, overrides);
    })
    {
    }
}
