using CsvHelper.Configuration;
using Flow.Domain.Transactions.Transfers;

namespace Flow.Infrastructure.IO.CSV.Transactions.Transfers;

internal class TransferKeyReader : CsvReader<TransferKey, TransferKeyRow>
{
    public TransferKeyReader(CsvConfiguration config) : base(config, r => (TransferKey)r)
    {
    }
}
