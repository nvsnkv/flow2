using Flow.Infrastructure.IO.Contract;
using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace Flow.Infrastructure.Configuration.UnitTests;

public class FlowConfigurationDtoShould
{
    [Fact]
    public void BeSerializableFromValidConfiguration()
    {
        var cfg = new ConfigurationBuilder().AddJsonFile("./test_valid_config.json").Build();

        var config = new FlowConfigurationDto(cfg.GetSection("flow"));

        config.Editor.Keys.Should().BeEquivalentTo(new[] { new SupportedFormat("csv"), new SupportedFormat("json") });
    }
}
