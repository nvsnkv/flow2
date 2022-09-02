using CommandLine;
using Flow.Hosts.Entrypoint.Commands;

[Verb("rates", HelpText = "Invokes rates management module. Please refer to `rates help` output for details")]
internal sealed class RatesCommand : CommandBase { }