namespace Flow.Infrastructure.Configuration.Contract;
public interface IFlowConfiguration
{
    public string? ConnectionString { get; }

    string? CultureCode { get; }

    string? NumberStyle { get; }

    string? DateStyle { get; }

    IDictionary<OldSupportedFormat, string>? Editor { get; }

    string? PluginsPath { get; }
}