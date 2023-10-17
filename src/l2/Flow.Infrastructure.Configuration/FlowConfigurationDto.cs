using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Contract;
using Microsoft.Extensions.Configuration;

namespace Flow.Infrastructure.Configuration;

internal class FlowConfigurationDto : IFlowConfiguration
{
    public FlowConfigurationDto(IConfigurationSection section)
    {
        section.Bind(this);
        Editor = new Dictionary<SupportedFormat, string>();
        var editors = section.GetSection(nameof(Editor));
        foreach (var child in editors.GetChildren())
        {
            if (!string.IsNullOrEmpty(child.Value))
            {
                Editor[new SupportedFormat(child.Key)] = child.Value;
            }
        }
    }

    public string? ConnectionString { get; set;  }

    public string? CultureCode { get; set; }

    public string? NumberStyle { get; set; }

    public string? DateStyle { get; set; }

    public IDictionary<SupportedFormat, string>? Editor { get; set; }

    public string? PluginsPath { get; set; }
}
