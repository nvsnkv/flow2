using System.Diagnostics;
using System.Globalization;
using System.Text;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Contract;

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
        if (outputPath == null) return waitForExit ? -1 : 0;

        var startInfo = config.Editor?.ContainsKey(format) ?? false
            ? new ProcessStartInfo(config.Editor[format], outputPath)
            : new ProcessStartInfo(outputPath);

        startInfo.UseShellExecute = true;


        var process = Process.Start(startInfo);

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
        var filename = $"temp.{command}.{DateTime.Now:s}.{slug}.{format.ToString().ToLower()}".Replace(":", "_");

        return Path.GetFullPath(filename);
    }

    protected StreamReader CreateReader(string? input)
    {
        var stream = string.IsNullOrEmpty(input)
            ? Console.OpenStandardInput()
            : File.OpenRead(input);

        return new StreamReader(stream, input != null ? Encoding.UTF8 : Encoding.GetEncoding(culture.TextInfo.OEMCodePage));
    }
}
