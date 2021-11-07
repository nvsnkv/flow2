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
        var builder = new ConfigurationBuilder();
        if (!string.IsNullOrEmpty(ConfigurationFile))
        {
            builder.AddJsonFile(ConfigurationFile, true);
        }

        config = builder
            .AddJsonFile( "appsettings.json", true)
            .AddJsonFile($"appsettings.Development.json", true)
            .AddEnvironmentVariables()
            .Build();
    }

    protected override void Load(ContainerBuilder builder)
    {
        builder.Register(_ => config.GetSection("flow").Get<FlowConfigurationDto>()).AsImplementedInterfaces();
        base.Load(builder);
    }
}