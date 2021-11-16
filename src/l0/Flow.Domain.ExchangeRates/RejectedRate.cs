using Flow.Domain.Common;

namespace Flow.Domain.ExchangeRates;

public class RejectedRate : RejectedEntity<ExchangeRate>
{
    public RejectedRate(ExchangeRate entity, IReadOnlyList<string> reasons) : base(entity, reasons)
    {
    }

    public ExchangeRate Rate => Entity;
}