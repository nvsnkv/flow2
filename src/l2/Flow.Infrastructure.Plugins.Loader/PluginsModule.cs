
using System.Reflection;
using Autofac;
using Flow.Application.Transactions.Contract;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Contract;
using Flow.Infrastructure.Plugins.Contract;
using Module = Autofac.Module;

namespace Flow.Infrastructure.Plugins.Loader
{
    public sealed class PluginsModule : Module
    {
        private static readonly Type[] SupportedTypes = { typeof(ITransferDetector), typeof(IFormatSpecificReader<>), typeof(IFormatSpecificWriter<>) };

        private readonly IFlowConfiguration config;
        private volatile int isInitialized;

        public PluginsModule(IFlowConfiguration config)
        {
            this.config = config;
        }

        private IEnumerable<IPlugin> LoadPlugins()
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
                        var ctx = new PluginLoadContext(path);
                        var assembly = ctx.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(path)));
                        var bootstrappers =
                            assembly.ExportedTypes.Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPluginsBootstrapper<>)));

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

        private IEnumerable<Type> GetInterfacesToRegister(Type pluginType)
        {
            return pluginType.GetInterfaces().Where(i => SupportedTypes.Contains(i) || i.IsGenericType && SupportedTypes.Contains(i.GetGenericTypeDefinition()));
        }

        protected override void Load(ContainerBuilder builder)
        {
            var plugins = LoadPlugins();

            foreach (var plugin in plugins)
            {
                foreach (var type in GetInterfacesToRegister(plugin.GetType()))
                {
                    builder.RegisterInstance(plugin).As(type);
                }

            }

            base.Load(builder);
        }
    }
}
