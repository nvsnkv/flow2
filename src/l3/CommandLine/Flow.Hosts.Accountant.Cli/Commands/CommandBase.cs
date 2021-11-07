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
        if (outputPath != null && (config.Editor?.ContainsKey(format) ?? false))
        {
            var process = Process.Start(new ProcessStartInfo(config.Editor[format], outputPath)
            {
                UseShellExecute = true
            });

            if (waitForExit && process != null)
            {
                await process.WaitForExitAsync();
                return process.ExitCode;
            }

            return 0;
        }

        return 0;
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

    private string GeneratePath(SupportedFormat format, string command, string slug)
    {
        return $"{command}.{DateTime.Now:s}.{slug}.{format.ToString().ToLower()}".Replace(":","_");
    }
}