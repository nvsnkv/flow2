using Autofac;
using Microsoft.Extensions.Configuration;

namespace Flow.Infrastructure.Configuration;

public sealed class FlowConfiguration : Module
{
    // ReSharper disable once InconsistentNaming
    public const string ENV_FLOW_CONFIG = "FLOW_CONFIG_FILE";

    public static readonly string? ConfigurationFile = Environment.GetEnvironmentVariable(ENV_FLOW_CONFIG);

    private readonly IConfiguration config;

    public FlowConfiguration()
    {
        config = new ConfigurationBuilder()
            .AddEnvironmentVariables().AddJsonFile(ConfigurationFile, true)
            .AddEnvironmentVariables()
            .Build();
    }

    protected override void Load(ContainerBuilder builder)
    {
        builder.Register(_ => config.GetSection("flow").Get<FlowConfigurationDto>()).AsImplementedInterfaces();
        base.Load(builder);
    }
}