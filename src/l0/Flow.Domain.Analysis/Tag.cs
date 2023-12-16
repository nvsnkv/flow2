namespace Flow.Domain.Analysis;

public sealed record Tag(string Name)
{
    public static Tag Invalid = new(string.Empty);

    public override string ToString()
    {
        return Name;
    }
}
