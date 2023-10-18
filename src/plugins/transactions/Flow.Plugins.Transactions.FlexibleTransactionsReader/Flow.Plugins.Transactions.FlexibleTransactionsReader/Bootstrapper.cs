using System.Globalization;
using Flow.Infrastructure.Plugins.Contract;
using Flow.Plugins.Transactions.FlexibleTransactionsReader.Settings;
using Microsoft.Extensions.Configuration;

namespace Flow.Plugins.Transactions.FlexibleTransactionsReader;

public sealed class Bootstrapper : IPluginsBootstrapper<IFlexibleTransactionsReader>
{
    public static readonly string ConfigurationFilePath = "mappings.json";

    public IEnumerable<IFlexibleTransactionsReader> GetPlugins()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile(ConfigurationFilePath)
            .AddEnvironmentVariables()
            .Build();

        return configuration.GetSection("TransactionReaderMappings")
            .GetChildren()
            .Select(c => new Impl.FlexibleTransactionsReader(CultureInfo.CurrentCulture, c.Key, c.Get<IReadOnlyCollection<MappingRule>>()));
    }
}
