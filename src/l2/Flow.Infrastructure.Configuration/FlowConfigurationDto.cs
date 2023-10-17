using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Contract;

namespace Flow.Infrastructure.Configuration;

internal class FlowConfigurationDto : IFlowConfiguration
{
    public string? ConnectionString { get; set;  }

    public string? CultureCode { get; set; }

    public string? NumberStyle { get; set; }

    public string? DateStyle { get; set; }

    public IDictionary<SupportedFormat, string>? Editor { get; set; }

    public string? PluginsPath { get; set; }
}
