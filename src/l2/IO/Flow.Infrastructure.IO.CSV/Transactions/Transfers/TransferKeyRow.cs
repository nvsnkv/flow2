using System.Diagnostics.CodeAnalysis;
using Flow.Domain.Transactions.Transfers;

namespace Flow.Infrastructure.IO.CSV.Transactions.Transfers;

[SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal class TransferKeyRow
{
    public long SOURCE { get; init; }

    public long SINK { get; init; }

    public static explicit operator TransferKey(TransferKeyRow row)
    {
        return new TransferKey(row.SOURCE, row.SINK);
    }

    public static explicit operator TransferKeyRow(TransferKey key)
    {
        return new TransferKeyRow
        {
            SINK = key.SinkKey,
            SOURCE = key.SourceKey
        };
    }
}