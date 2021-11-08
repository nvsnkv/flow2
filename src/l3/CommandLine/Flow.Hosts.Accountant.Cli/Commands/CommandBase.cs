using System.Diagnostics;
using Flow.Infrastructure.Configuration.Contract;

namespace Flow.Hosts.Accountant.Cli.Commands;

internal abstract class CommandBase
{
    private readonly IFlowConfiguration config;

    protected CommandBase(IFlowConfiguration config)
    {
        this.config = config;
    }
    
    protected async Task<int> TryStartEditor(string? outputPath, SupportedFormat format, bool waitForExit)
    {
        if (outputPath == null || !(config.Editor?.ContainsKey(format) ?? false)) return waitForExit ? -1 : 0;

        var process = Process.Start(new ProcessStartInfo(config.Editor[format], outputPath)
        {
            UseShellExecute = true
        });

        if (!waitForExit) return 0;
        if (process == null) return -1;

        await process.WaitForExitAsync();
        return process.ExitCode;
    }

    protected StreamWriter CreateWriter(string? output)
    {
        var stream = output == null
            ? Console.OpenStandardOutput()
            : File.OpenWrite(output);

        return new StreamWriter(stream);
    }

    protected string? GetFallbackOutputPath(SupportedFormat format, string command, string slug)
    {

        if (config.Editor?.ContainsKey(format) ?? false)
        {
            return GeneratePath(format, command, slug);
        }

        return null;
    }

    protected StreamReader CreateReader(string? input)
    {
        var stream = string.IsNullOrEmpty(input)
            ? Console.OpenStandardInput()
            : File.OpenRead(input);

        return new StreamReader(stream);
    }

    private string GeneratePath(SupportedFormat format, string command, string slug)
    {
        return $"{command}.{DateTime.Now:s}.{slug}.{format.ToString().ToLower()}".Replace(":", "_");
    }
}