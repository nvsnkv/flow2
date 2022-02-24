using System.Collections;

namespace Flow.Domain.Analysis;

public class Vector : IReadOnlyList<string>
{
    public static Vector Empty { get; } = new(Enumerable.Empty<string>());

    private readonly List<string> values;
    
    public Vector(IEnumerable<string>? values)
    {
        this.values = values?.ToList() ?? Empty.values;
    }

    public int Count => values.Count;

    public string this[int i] => values[i];

    protected bool Equals(Vector other)
    {
        if (Count != other.Count)
        {
            return false;
        }

        for (var i = 0; i < Count; i++)
        {
            if (this[i] != other[i])
            {
                return false;
            }
        }

        return true;
    }

    public IEnumerator<string> GetEnumerator()
    {
        return values.GetEnumerator();
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Vector)obj);
    }

    public override int GetHashCode() => values.Aggregate(0, (s, i) => HashCode.Combine(s, i.GetHashCode()));
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public override string ToString()
    {
        return string.Join(", ", values);
    }

    public static bool operator ==(Vector? left, Vector? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Vector? left, Vector? right)
    {
        return !Equals(left, right);
    }

    public Vector PadRight(int dimensionsCount)
    {
        return new Vector(PadParts(dimensionsCount));
    }

    private IEnumerable<string> PadParts(int dimensionsCount)
    {
        int i = 0;
        foreach (var value in values)
        {
            yield return value;
            i++;
        }

        while (i < dimensionsCount)
        {
            yield return string.Empty;
            i++;
        }
    }
}