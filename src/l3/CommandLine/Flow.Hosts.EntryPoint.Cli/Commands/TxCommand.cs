using CommandLine;
using JetBrains.Annotations;

namespace Flow.Hosts.EntryPoint.Cli.Commands;

[Verb("tx", HelpText = "Invokes transactions management module. Please refer to `tx help` output for details"), UsedImplicitly]
internal sealed class TxCommand { }