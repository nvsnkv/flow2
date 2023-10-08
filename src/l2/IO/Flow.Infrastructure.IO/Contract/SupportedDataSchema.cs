namespace Flow.Infrastructure.IO.Contract;

public sealed class SupportedDataSchema
{
    public static SupportedDataSchema Default { get; } = new SupportedDataSchema("default");

    private readonly string name;

    public SupportedDataSchema(string name)
    {
        this.name = name;
    }

    private bool Equals(SupportedDataSchema other)
    {
        return name == other.name;
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is SupportedDataSchema other && Equals(other);
    }

    public override int GetHashCode()
    {
        return name.GetHashCode();
    }

    public override string ToString()
    {
        return name;
    }

    public static explicit operator SupportedDataSchema(string schema)
    {
        return new(schema);
    }
}
