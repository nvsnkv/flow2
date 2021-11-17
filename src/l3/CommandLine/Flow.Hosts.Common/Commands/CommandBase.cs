using System.Diagnostics;
using System.Globalization;
using System.Text;
using Flow.Infrastructure.Configuration.Contract;

namespace Flow.Hosts.Common.Commands;

public abstract class CommandBase
{
    private readonly IFlowConfiguration config;
    private readonly CultureInfo culture;

    protected CommandBase(IFlowConfiguration config)
    {
        this.config = config;
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        culture = CultureInfo.GetCultures(CultureTypes.AllCultures).FirstOrDefault(c => c.Name == config.CultureCode) ?? CultureInfo.CurrentCulture;
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

        return new StreamWriter(stream, output != null ? Encoding.UTF8 : Encoding.GetEncoding(culture.TextInfo.OEMCodePage));
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

        return new StreamReader(stream, input != null ? Encoding.UTF8 : Encoding.GetEncoding(culture.TextInfo.OEMCodePage));
    }

    private string GeneratePath(SupportedFormat format, string command, string slug)
    {
        return $"{command}.{DateTime.Now:s}.{slug}.{format.ToString().ToLower()}".Replace(":", "_");
    }
}