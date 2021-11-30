﻿namespace Flow.Domain.Analysis;

public class Range
{
    public Range(DateTime start, DateTime end)
    {
        End = end;
        Start = start;
    }

    public DateTime Start { get; }

    public DateTime End { get; }

    public string Alias { get; set; }

    protected bool Equals(Range other)
    {
        return Start.Equals(other.Start) && End.Equals(other.End);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Range)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Start, End);
    }

    public static bool operator ==(Range? left, Range? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Range? left, Range? right)
    {
        return !Equals(left, right);
    }

    public void Deconstruct(out DateTime start, out DateTime end)
    {
        start = Start;
        end = End;
    }

    public static implicit operator Range((DateTime, DateTime) range)
    {
        var (start, end) = range;
        return new(start, end);
    }

    public override string ToString() => $"[{Start:yyyy-MM-dd}, {End:yyyy-MM-dd})";
}