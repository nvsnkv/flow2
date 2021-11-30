namespace Flow.Domain.Analysis;

public class DailyOffset : Offset
{
    private readonly int days;

    public DailyOffset(int days)
    {
        this.days = days;
    }

    public override DateTime ApplyTo(DateTime time)
    {
        return time.AddDays(days);
    }
}