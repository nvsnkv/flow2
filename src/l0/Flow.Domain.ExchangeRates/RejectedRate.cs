using Flow.Domain.Common;

namespace Flow.Domain.ExchangeRates;

public class RejectedRate : RejectedEntity<ExchangeRate>
{
    public RejectedRate(ExchangeRate rate, params string[] reasons) : base(rate, reasons.ToList().AsReadOnly())
    {
    }

    public RejectedRate(ExchangeRate rate, IReadOnlyList<string> reasons) : base(rate, reasons)
    {
    }

    public ExchangeRate Rate => Entity;
}