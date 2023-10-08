namespace Flow.Infrastructure.Plugins.Transactions.Contract;

public interface IPluginsBootstrapper<out T> where T : IPlugin
{
    IEnumerable<T> GetPlugins();
}
