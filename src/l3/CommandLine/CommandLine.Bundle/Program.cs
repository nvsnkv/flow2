using System.Globalization;
using CommandLine;
using Flow.Hosts.Common;
using JetBrains.Annotations;

var parser = ParserHelper.Create(CultureInfo.CurrentCulture);

var arguments = parser.ParseArguments<RegisterArgs, UnregisterArgs>(args);
return await arguments.MapResult(
    (RegisterArgs _) => Task.FromResult(-1),
    (UnregisterArgs _) => Task.FromResult(-1),
    async errs => await ParserHelper.HandleUnparsed(errs, arguments)
);


[Verb("register", HelpText = "Adds application folder to PATH to make flow tools available in CLI and registers the settings file."), UsedImplicitly]
internal class RegisterArgs
{

}

[Verb("unregister", HelpText = "Removes application folder from PATH and removes settings file registration"), UsedImplicitly]
internal class UnregisterArgs
{

}