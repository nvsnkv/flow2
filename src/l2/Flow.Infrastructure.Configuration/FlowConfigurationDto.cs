using Flow.Infrastructure.Configuration.Contract;

namespace Flow.Infrastructure.Configuration;

internal class FlowConfigurationDto : IFlowConfiguration
{
    public string? ConnectionString { get; set;  }

    public string? CultureCode { get; set; }
}