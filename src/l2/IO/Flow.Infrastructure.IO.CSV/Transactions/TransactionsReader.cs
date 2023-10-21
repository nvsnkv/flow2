using CsvHelper.Configuration;
using Flow.Application.Transactions.Contract;
using Flow.Domain.Transactions;

namespace Flow.Infrastructure.IO.CSV.Transactions;

internal class TransactionsReader : CsvReader<IncomingTransaction,TransactionRow>
{
    public TransactionsReader(CsvConfiguration config) : base(config, (TransactionRow r) =>
    {
        var (transaction, overrides) = r;
        return new(transaction, overrides);
    })
    {
    }
}
