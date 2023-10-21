namespace Flow.Infrastructure.Plugins.Contract;

public interface IPluginsBootstrapper
{
    IEnumerable<IPlugin> GetPlugins();
}
