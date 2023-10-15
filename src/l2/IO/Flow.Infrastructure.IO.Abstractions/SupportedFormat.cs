namespace Flow.Infrastructure.IO.Abstractions;

public sealed record SupportedFormat(string Name)
{
    public static explicit operator SupportedFormat(string name) => new(name);

    public override string ToString() => Name;
}
