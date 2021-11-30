namespace Flow.Domain.Analysis;

public class MonthlyOffset : Offset
{
    public override DateTime ApplyTo(DateTime time)
    {
        return time.AddMonths(1);
    }
}