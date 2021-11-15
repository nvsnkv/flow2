namespace Flow.Domain.ExchangeRates;

public class ExchangeRate : ExchangeRateRequest
{
    public ExchangeRate(string from, string to, DateTime date, decimal rate) : base(from, to, date)
    {
        Rate = rate;
    }

    public decimal Rate { get; }

    protected bool Equals(ExchangeRate other)
    {
        return base.Equals(other) && Rate == other.Rate;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((ExchangeRate)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Rate);
    }
}
