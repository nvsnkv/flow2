using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using Flow.Infrastructure.Plugins.Contract;
using Flow.Plugins.Transactions.FlexibleTransactionsReader.Settings;
using Microsoft.Extensions.Configuration;

[assembly:InternalsVisibleTo("Flow.Plugins.Transactions.FlexibleTransactionsReader.Tests")]
namespace Flow.Plugins.Transactions.FlexibleTransactionsReader;

public sealed class Bootstrapper : IPluginsBootstrapper
{
    private readonly string _configFilePath;
    private readonly CultureInfo _culture;
    private static readonly string ConfigurationFilePath = "mappings.json";

    public Bootstrapper() : this(ConfigurationFilePath, CultureInfo.CurrentCulture)
    {
    }

    public Bootstrapper(string configFilePath, CultureInfo culture)
    {
        _configFilePath = configFilePath;
        _culture = culture;
    }

    public IEnumerable<IPlugin> GetPlugins()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile(_configFilePath, optional: true)
            .AddEnvironmentVariables()
            .Build();

        return configuration.GetSection("TransactionReaderMappings")
            .GetChildren()
            .Select(c => new Impl.FlexibleTransactionsReader(_culture, c.Key, c.Get<IReadOnlyCollection<MappingRule>>()));
    }
}
