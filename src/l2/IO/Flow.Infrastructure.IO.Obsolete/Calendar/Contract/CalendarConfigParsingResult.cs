using Flow.Domain.Analysis.Setup;

namespace Flow.Infrastructure.IO.Calendar.Contract;

public class CalendarConfigParsingResult
{
    public CalendarConfigParsingResult(CalendarConfig? config)
    {
        Config = config;
        Errors = Enumerable.Empty<string>();
    }

    public CalendarConfigParsingResult(IEnumerable<string> errors)
    {
        Errors = errors;
    }

    public CalendarConfigParsingResult(params string[] errors)
    {
        Errors = errors;
    }

    public CalendarConfig? Config { get; }

    public bool Successful => Config != null;

    public IEnumerable<string> Errors { get; }
}