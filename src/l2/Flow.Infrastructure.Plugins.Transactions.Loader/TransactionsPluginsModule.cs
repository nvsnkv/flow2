
using System.Reflection;
using Autofac;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.Plugins.Transactions.Contract;
using Module = Autofac.Module;

namespace Flow.Infrastructure.Plugins.Transactions.Loader
{
    public sealed class TransactionsPluginsModule : Module
    {
        private readonly IFlowConfiguration config;
        private volatile int isInitialized;

        public TransactionsPluginsModule(IFlowConfiguration config)
        {
            this.config = config;
        }

        private IEnumerable<IPlugin> LoadSupportedPlugins(params Type[] supportedTypes)
        {
            if (string.IsNullOrEmpty(config.PluginsPath) || !Directory.Exists(config.PluginsPath))
            {
                return Enumerable.Empty<IPlugin>();
            }

            if (Interlocked.CompareExchange(ref isInitialized, 1, 0) != 0)
            {
                return Enumerable.Empty<IPlugin>();
            }

            supportedTypes = supportedTypes.Select(t => typeof(IPluginsBootstrapper<>).MakeGenericType(t)).ToArray();

            var result = new List<IPlugin>();

            try
            {
                foreach (var dir in Directory.EnumerateDirectories(config.PluginsPath))
                {
                    var libraryPaths = Directory.EnumerateFiles(dir, "*.dll").ToList();
                    foreach (var path in libraryPaths)
                    {
                        var ctx = new PluginLoadContext(path);
                        var assembly = ctx.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(path)));
                        var bootstrappers =
                            assembly.ExportedTypes.Where(t => supportedTypes.Any(t.IsAssignableTo));

                        foreach (var bootstrapper in bootstrappers)
                        {
                            if (Activator.CreateInstance(bootstrapper) is IPluginsBootstrapper<IPlugin> instance)
                            {
                                result.AddRange(instance.GetPlugins());
                            }
                        }
                    }
                }
            }
            catch
            {
                isInitialized = 0;
                throw;
            }

            return result;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var plugins = LoadSupportedPlugins(typeof(ITransferDetectionPlugin), typeof(ITransactionsReaderPlugin));

            foreach (var plugin in plugins)
            {
                switch (plugin)
                {
                    case ITransferDetectionPlugin detectionPlugin:
                        builder.RegisterInstance(new TransferDetectorAdapter(detectionPlugin)).AsImplementedInterfaces();
                        break;
                    case ITransactionsReaderPlugin readerPlugin:
                        builder.RegisterInstance(new TransactionsReaderAdapter(readerPlugin)).AsImplementedInterfaces();
                        break;
                }
            }

            base.Load(builder);
        }
    }
}
