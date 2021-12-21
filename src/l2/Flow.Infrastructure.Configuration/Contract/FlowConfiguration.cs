﻿using Autofac;
using Microsoft.Extensions.Configuration;

namespace Flow.Infrastructure.Configuration.Contract;

public sealed class FlowConfiguration : Module
{
    // ReSharper disable once InconsistentNaming
    public const string ENV_FLOW_CONFIG = "FLOW_CONFIG_FILE";

    public static readonly string? ConfigurationFile = Environment.GetEnvironmentVariable(ENV_FLOW_CONFIG, EnvironmentVariableTarget.User);

    private readonly IConfiguration config;

    public FlowConfiguration()
    {
        var builder = new ConfigurationBuilder();

        builder
            .AddJsonFile("appsettings.json", true)
            .AddJsonFile($"appsettings.Development.json", true);

        if (!string.IsNullOrEmpty(ConfigurationFile))
        {
            builder.AddJsonFile(ConfigurationFile, true);
        }

        builder.AddEnvironmentVariables();

        config = builder.Build();
    }

    protected override void Load(ContainerBuilder builder)
    {
        builder.Register(_ => config.GetSection("flow").Get<FlowConfigurationDto>()).AsImplementedInterfaces();
        base.Load(builder);
    }
}