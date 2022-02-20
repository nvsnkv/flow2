namespace Flow.Infrastructure.IO.Calendar;

internal class JsonCalendarConfig
{
    public List<string>? Dimensions { get; set; }

    public List<JsonSeriesConfig>? Series { get; set; }
}