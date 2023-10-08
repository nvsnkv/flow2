namespace Flow.Infrastructure.IO.Calendar.Contract;

public interface ICalendarConfigParser
{
    Task<CalendarConfigParsingResult> ParseFromStream(StreamReader reader, CancellationToken ct);
}