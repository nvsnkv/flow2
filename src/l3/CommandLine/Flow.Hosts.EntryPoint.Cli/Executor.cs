using System.Reflection;

namespace Flow.Hosts.EntryPoint.Cli;

internal sealed class Executor
{
    private readonly CancellationToken cancellationToken;
    private readonly TextWriter errorsWriter;

    public Executor(CancellationToken cancellationToken, TextWriter errorsWriter)
    {
        this.cancellationToken = cancellationToken;
        this.errorsWriter = errorsWriter;
    }

    public async Task<int> Execute(string assemblyName, IEnumerable<string> args)
    {
        var filename = Path.Combine(".", $"{assemblyName}.dll");

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

        if (assembly.EntryPoint.Invoke(null, new object[] { args.ToArray() }) is not Task<int> task)
        {
            await errorsWriter.WriteLineAsync("Entry point did not return awaitable result! Action was started but no idea when it will be finished...");
            return -7;
        }

        return await task;
    }
}