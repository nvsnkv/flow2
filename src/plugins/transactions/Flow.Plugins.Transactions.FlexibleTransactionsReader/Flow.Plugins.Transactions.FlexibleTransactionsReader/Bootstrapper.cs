using System.Globalization;
using System.Runtime.CompilerServices;
using Flow.Infrastructure.Plugins.Contract;
using Flow.Plugins.Transactions.FlexibleTransactionsReader.Settings;
using Microsoft.Extensions.Configuration;

[assembly:InternalsVisibleTo("Flow.Plugins.Transactions.FlexibleTransactionsReader.Tests")]
namespace Flow.Plugins.Transactions.FlexibleTransactionsReader;

public sealed class Bootstrapper : IPluginsBootstrapper
{
    private readonly string _configFilePath;
    private static readonly string ConfigurationFilePath = "mappings.json";

    public Bootstrapper() : this(ConfigurationFilePath)
    {
    }

    public Bootstrapper(string configFilePath)
    {
        _configFilePath = configFilePath;
    }

    public IEnumerable<IPlugin> GetPlugins()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile(_configFilePath, optional: true)
            .AddEnvironmentVariables()
            .Build();

        return configuration.GetSection("TransactionReaderMappings")
            .GetChildren()
            .Select(c => new Impl.FlexibleTransactionsReader(CultureInfo.CurrentCulture, c.Key, c.Get<IReadOnlyCollection<MappingRule>>()));
    }
}
