namespace Flow.Hosts.Common;

public class ConsoleCancellationHandler
{
    private readonly CancellationTokenSource source = new();

    public CancellationToken Token => source.Token;

    public ConsoleCancellationHandler()
    {
        Console.CancelKeyPress += (_, _) =>
        {
            source.Cancel(true);
            Environment.Exit(2);
        };
    }
}