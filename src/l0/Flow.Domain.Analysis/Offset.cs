namespace Flow.Domain.Analysis;

public abstract class Offset
{
    public abstract DateTime ApplyTo(DateTime time);
}