using System;

namespace Flow.Domain.Transactions;

public class Overrides
{
    public Overrides(string? category, string? title, string? comment)
    {
        Category = category;
        Title = title;
        Comment = comment;
    }

    public string? Comment { get; }

    public string? Title { get; }

    public string? Category { get; }

    protected bool Equals(Overrides other)
    {
        return Comment == other.Comment && Title == other.Title && Category == other.Category;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Overrides)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Comment, Title, Category);
    }

    public static bool operator ==(Overrides? left, Overrides? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Overrides? left, Overrides? right)
    {
        return !Equals(left, right);
    }
}