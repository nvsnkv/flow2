
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

        private IEnumerable<IPlugin> LoadTransferDetectionPlugins()
        {
            if (string.IsNullOrEmpty(config.PluginsPath) || !Directory.Exists(config.PluginsPath))
            {
                return Enumerable.Empty<IPlugin>();
            }

            if (Interlocked.CompareExchange(ref isInitialized, 1, 0) != 0)
            {
                return Enumerable.Empty<IPlugin>();
            }

            var result = new List<IPlugin>();

            try
            {
                foreach (var dir in Directory.EnumerateDirectories(config.PluginsPath))
                {
                    var libraryPaths = Directory.EnumerateFiles(dir, "*.dll").ToList();
                    foreach (var path in libraryPaths)
                    {
                        var ctx = new PluginLoadContext(dir);
                        var assembly = ctx.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(path)));
                        var plugins =
                            assembly.ExportedTypes.Where(t => t.IsAssignableTo(typeof(ITransferDetectionPlugin)));

                        foreach (var plugin in plugins)
                        {
                            if (Activator.CreateInstance(plugin) is IPlugin instance)
                            {
                                result.Add(instance);
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
            var plugins = LoadTransferDetectionPlugins();

            foreach (var plugin in plugins)
            {
                if (plugin is ITransferDetectionPlugin detectionPlugin)
                {
                    builder.RegisterInstance(new TransferDetectorAdapter(detectionPlugin)).AsImplementedInterfaces();
                }
            }

            base.Load(builder);
        }
    }
}