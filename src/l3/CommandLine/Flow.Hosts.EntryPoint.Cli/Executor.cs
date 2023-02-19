using System.Reflection;

namespace Flow.Hosts.EntryPoint.Cli;

internal sealed class Executor
{
    private readonly TextWriter errorsWriter;
    private readonly IDictionary<string, MethodInfo> entryPoints = new Dictionary<string, MethodInfo>();

    public Executor(TextWriter errorsWriter)
    {
        this.errorsWriter = errorsWriter;
    }

    public async Task<int> Execute(string assemblyName, IEnumerable<string> args)
    {
        if (!entryPoints.ContainsKey(assemblyName))
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) 
                      ?? throw new NotSupportedException("Failed to detect current location of the current assembly!");

            var filename = Path.Combine(dir, $"{assemblyName}.dll");

            Assembly assembly;
            try
            {
                assembly = Assembly.LoadFrom(filename);
            }
            catch (Exception e)
            {
                await errorsWriter.WriteAsync($"Failed to load assembly {assemblyName}: {e}");
                return -3;
            }

            if (assembly.EntryPoint == null)
            {
                await errorsWriter.WriteLineAsync($"Failed to find entry point for assembly {assemblyName}!");
                return -5;
            }

            entryPoints.Add(assemblyName, assembly.EntryPoint);
        }

        var entryPoint = entryPoints[assemblyName];

        if (entryPoint.Invoke(null, new object[] { args.ToArray() }) is not int result)
        {
            await errorsWriter.WriteLineAsync("Entry point did not return integer result!");
            return -7;
        }

        return result;
    }
}