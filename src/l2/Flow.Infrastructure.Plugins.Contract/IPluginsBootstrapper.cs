namespace Flow.Infrastructure.Plugins.Contract;

public interface IPluginsBootstrapper<out T> where T : IPlugin
{
    IEnumerable<T> GetPlugins();
}
