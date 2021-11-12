using System.Collections.Generic;
using System.Linq;

namespace Flow.Domain.Transactions.Transfers;

public class RejectedTransferKey : RejectedEntity<TransferKey>
{
    public RejectedTransferKey(TransferKey entity, params string[] reasons) : this(entity, reasons.ToList().AsReadOnly())
    {

    }

    public RejectedTransferKey(TransferKey entity, IReadOnlyList<string> reasons) : base(entity, reasons)
    {
    }
}