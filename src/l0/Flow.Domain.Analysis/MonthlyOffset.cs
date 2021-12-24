namespace Flow.Domain.Analysis;

public class MonthlyOffset : Offset
{
    public override DateTime ApplyTo(DateTime time)
    {
        return time.AddMonths(1);
    }

    public override string GetAliasFor(Range range)
    {
        return range.Start.ToLocalTime().ToString("MMM");
    }
}