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
}