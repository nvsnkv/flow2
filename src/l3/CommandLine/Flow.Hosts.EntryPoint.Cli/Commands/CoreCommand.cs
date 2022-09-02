using CommandLine;

namespace Flow.Hosts.EntryPoint.Cli.Commands;

[Verb("core", HelpText = "Invokes analysis module. Please refer to `core help` output for details")]
internal sealed class CoreCommand : CommandWithOptionalArgs { }