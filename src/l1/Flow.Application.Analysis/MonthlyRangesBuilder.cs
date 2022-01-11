using Range = Flow.Domain.Analysis.Range;

namespace Flow.Application.Analysis;

internal class MonthlyRangesBuilder
{
    private readonly DateTime from;
    private readonly DateTime till;
    
    public MonthlyRangesBuilder(DateTime from, DateTime till)
    {
        this.from = from;
        this.till = till;
    }
    public IEnumerable<Range> GetRanges()
    {
        var end = from.ToLocalTime();

        do
        {
            var start = end;
            end = start.AddMonths(1);
            var range = new Range(start.ToUniversalTime(), till <= end ? till : end.ToUniversalTime())
            {
                Alias = start.ToString("MMM")
            };

            yield return range;
        } while (till > end);

    }
}