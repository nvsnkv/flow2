using CsvHelper.Configuration;
using Flow.Domain.Transactions.Transfers;

namespace Flow.Infrastructure.IO.CSV.Transactions.Transfers;

internal class TransferRejectionsWriter : CsvRejectionsWriter<RejectedTransferKey, TransferKey, TransferKeyRow>
{
    public TransferRejectionsWriter(CsvConfiguration config) : base(config, k => (TransferKeyRow)k.TransferKey)
    {
    }
}
