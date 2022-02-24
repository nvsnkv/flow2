namespace Flow.Infrastructure.IO.Calendar;

internal class JsonSeriesConfig
{
    public List<string>? Measurement { get; set; }

    public List<string>? Rules { get; set; }

    public string? Rule { get; set; }

    public List<JsonSeriesConfig>? SubSeries { get; set; }
}