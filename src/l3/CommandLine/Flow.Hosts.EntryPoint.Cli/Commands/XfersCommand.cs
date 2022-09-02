using CommandLine;

namespace Flow.Hosts.EntryPoint.Cli.Commands;

[Verb("xfers", HelpText = "Invokes transfers management module. Please refer to `xfers help` output for details")]
internal sealed class XfersCommand : CommandWithOptionalArgs { }