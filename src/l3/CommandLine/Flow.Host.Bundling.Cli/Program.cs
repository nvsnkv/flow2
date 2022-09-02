using System.Diagnostics;
using System.Globalization;
using CommandLine;
using Flow.Hosts.Common;
using Flow.Infrastructure.Configuration.Contract;
using JetBrains.Annotations;

var parser = ParserHelper.Create(CultureInfo.CurrentCulture);

var arguments = parser.ParseArguments<RegisterArgs, UnregisterArgs>(args);
return await arguments.MapResult(
    (RegisterArgs arg) =>
    {
        if (arg.Verbose) Console.WriteLine("Registering Flow CLI Bundle...");
        var path = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User) ?? string.Empty;
        path += $";{Environment.CurrentDirectory}";
        Environment.SetEnvironmentVariable("PATH", path, EnvironmentVariableTarget.User);
        if (arg.Verbose) Console.WriteLine("PATH updated.");

        Environment.SetEnvironmentVariable(FlowConfiguration.ENV_FLOW_CONFIG, Path.Combine(Environment.CurrentDirectory, "appsettings.json"), EnvironmentVariableTarget.User);
        if (arg.Verbose) Console.WriteLine($"{FlowConfiguration.ENV_FLOW_CONFIG} set.");

        if (arg.Verbose) Console.WriteLine("Bundle registered!");
        return Task.FromResult(0);
    },

    (UnregisterArgs arg) =>
    {
        if (arg.Verbose) Console.WriteLine("Unregistering current bundle...");
        Environment.SetEnvironmentVariable(FlowConfiguration.ENV_FLOW_CONFIG, null, EnvironmentVariableTarget.User);
        if (arg.Verbose) Console.WriteLine($"{FlowConfiguration.ENV_FLOW_CONFIG} unset");

        var path = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User) ?? string.Empty;
        var parts = path.Split(';', StringSplitOptions.RemoveEmptyEntries);
        path = string.Join(';', parts.Where(p => p != Environment.CurrentDirectory));
        Environment.SetEnvironmentVariable("PATH", path, EnvironmentVariableTarget.User);
        if (arg.Verbose) Console.WriteLine("PATH updated");

        if (arg.Verbose) Console.WriteLine("Bundle unregistered!");
        return Task.FromResult(0);
    },
    async errs => await ParserHelper.HandleUnparsed(errs, arguments)
);


[Verb("register", HelpText = "Adds application folder to PATH to make flow tools available in CLI and registers the settings file"), UsedImplicitly]
internal class RegisterArgs
{
    [Option('v', "verbose", Required = false, Default = false, HelpText = "Verbose output")]
    public bool Verbose { get; [UsedImplicitly] set; }
}

[Verb("unregister", HelpText = "Removes application folder from PATH and removes settings file registration"), UsedImplicitly]
internal class UnregisterArgs
{
    [Option('v', "verbose", Required = false, Default = false, HelpText = "Verbose output")]
    public bool Verbose { get; [UsedImplicitly] set; }
}