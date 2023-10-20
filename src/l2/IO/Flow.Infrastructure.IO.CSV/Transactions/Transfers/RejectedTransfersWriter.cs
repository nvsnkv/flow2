using CsvHelper.Configuration;
using Flow.Domain.Transactions.Transfers;

namespace Flow.Infrastructure.IO.CSV.Transactions.Transfers;

internal class RejectedTransfersWriter : CsvRejectionsWriter<RejectedTransferKey, TransferKey, TransferKeyRow>
{
    public RejectedTransfersWriter(CsvConfiguration config) : base(config, k => (TransferKeyRow)k.TransferKey)
    {
    }
}
