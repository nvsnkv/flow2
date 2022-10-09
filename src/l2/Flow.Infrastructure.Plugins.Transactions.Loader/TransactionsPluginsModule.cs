
using System.Reflection;
using Autofac;
using Flow.Infrastructure.Configuration.Contract;
using System.Runtime.InteropServices;
using Flow.Infrastructure.Plugins.Transactions.Contract;
using Module = Autofac.Module;

namespace Flow.Infrastructure.Plugins.Transactions.Loader
{
    public sealed class TransactionsPluginsModule : Module
    {
        private readonly IFlowConfiguration config;

        TransactionsPluginsModule(IFlowConfiguration config)
        {
            this.config = config;
        }

        public async Task InitializePlugins(CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(config.PluginsPath) || !Directory.Exists(config.PluginsPath))
            {
                return;
            }

            var bclPaths = Directory.EnumerateFiles(RuntimeEnvironment.GetRuntimeDirectory(), "*.dll").ToList();

            foreach (var dir in Directory.EnumerateDirectories(config.PluginsPath))
            {
                var libraryPaths = Directory.EnumerateFiles(dir, "*.dll").ToList();
                    throw new NotImplementedException("Load me and map types!");
            
            }
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
        }
    }
}