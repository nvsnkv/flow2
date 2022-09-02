namespace Flow.Hosts.EntryPoint.Cli;

internal sealed class Executor
{
    private readonly CancellationToken cancellationToken;

    public Executor(CancellationToken cancellationToken)
    {
        this.cancellationToken = cancellationToken;
    }

    public async Task<int> Execute(string assembly, IEnumerable<string> args)
    {
        Console.WriteLine($"{assembly}: {args}");
        return 0;
    }
}