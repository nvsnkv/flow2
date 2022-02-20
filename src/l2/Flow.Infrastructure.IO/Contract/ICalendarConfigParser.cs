namespace Flow.Infrastructure.IO.Contract;

public interface ICalendarConfigParser
{
    Task<CalendarConfigParsingResult> ParseFromStream(StreamReader reader, CancellationToken ct);
}