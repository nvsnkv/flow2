using System.Collections.Immutable;
using System.Reflection;
using Autofac;
using Flow.Application.Transactions.Contract;
using Flow.Domain.ExchangeRates;
using Flow.Domain.Transactions;
using Flow.Domain.Transactions.Transfers;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Collections;
using Flow.Infrastructure.IO.Contract;
using Flow.Infrastructure.Plugins.Contract;
using Flow.Infrastructure.Plugins.Loader;
using Transaction = System.Transactions.Transaction;

namespace Flow.Hosts.Bundling.Cli;

internal class InstallationDebugger
{
    public async Task<int> PrintoutDebugInfo()
    {
        try
        {
            await Console.Out.WriteLineAsync("Locations");
            await Console.Out.WriteLineAsync($"  flow binaries: {Assembly.GetEntryAssembly()?.Location}");
            await Console.Out.WriteLineAsync($"  flow settings path: {Environment.GetEnvironmentVariable(FlowConfiguration.ENV_FLOW_CONFIG)}");
            await Console.Out.WriteLineAsync();

            var builder = new ContainerBuilder();
            var flowConfigurationModule = new FlowConfiguration();

            builder.RegisterModule(flowConfigurationModule);
            var container = builder.Build();

            var config = container.Resolve<IFlowConfiguration>();
            await Console.Out.WriteLineAsync("Loaded configuration");
            await Console.Out.WriteLineAsync($"  cultureCode: {config.CultureCode}");
            await Console.Out.WriteLineAsync($"  numberStyle: {config.NumberStyle}");
            await Console.Out.WriteLineAsync($"  dateStyle: {config.DateStyle}");
            await Console.Out.WriteLineAsync($"  pluginsPath: {config.PluginsPath}");
            await Console.Out.WriteLineAsync("  editors:");
            foreach (var pair in config.Editor ?? ImmutableDictionary<SupportedFormat, string>.Empty)
            {
                await Console.Out.WriteLineAsync($"   {pair.Key}:{pair.Value}");
            }
            await Console.Out.WriteLineAsync();

            builder = new ContainerBuilder();
            builder.RegisterModule(flowConfigurationModule)
                .RegisterModule(new FlowIO(config))
                .RegisterModule(new PluginsModule(config));

            container = builder.Build();

            await Console.Out.WriteLineAsync("Supported formats");

            await PrintoutFormatsInfo(container);
            await Console.Out.WriteLineAsync();

            await Console.Out.WriteLineAsync("Loaded plugins:");

            var plugins = container.ComponentRegistry.Registrations
                .Where(r => r.Activator.LimitType.IsAssignableTo(typeof(IPlugin)))
                .Select(r => r.Activator.LimitType);

            foreach (var plugin in plugins)
            {
                await Console.Out.WriteLineAsync($"  {plugin.Name}: {plugin.Assembly.Location}");
            }

            return 0;
        }
        catch (Exception e)
        {
            await Console.Error.WriteLineAsync(e.ToString());
            return -1;
        }
    }

    private static readonly Type[] SupportedTypes =
    {
        typeof(Transaction), typeof(IncomingTransaction), typeof(RecordedTransaction), typeof(RejectedTransaction),
        typeof(TransferKey), typeof(Transfer), typeof(RejectedTransferKey),
        typeof(ExchangeRate), typeof(RejectedRate)
    };

    private static async Task PrintoutFormatsInfo(IComponentContext container)
    {
        foreach (var type in SupportedTypes)
        {
            var readers = container.Resolve(typeof(IReaders<>).MakeGenericType(type));
            var writers = container.Resolve(typeof(IWriters<>).MakeGenericType(type));

            var methodInfo = readers.GetType().GetMethod("GetKnownFormats") ?? throw new InvalidOperationException("supplied reader does not have method called 'GetFormatInfo'!");
            var readingFormats = methodInfo.Invoke(readers, null) as IEnumerable<SupportedFormat>;

            methodInfo = writers.GetType().GetMethod("GetKnownFormats") ?? throw new InvalidOperationException("supplied writer does not have method called 'GetFormatInfo'!");;
            var writingFormats = methodInfo.Invoke(writers, null) as IEnumerable<SupportedFormat>;

            await Console.Out.WriteAsync($"  {type.Name} - ");
            await Console.Out.WriteAsync(readingFormats?.Aggregate("read: ", (s, format) => s + format.Name + ", "));
            await Console.Out.WriteAsync(writingFormats?.Aggregate("write: ", (s, format) => s + format.Name + ", "));
            await Console.Out.WriteLineAsync();
        }
    }
}
