using CommandLine;
using Flow.Hosts.Entrypoint.Commands;

[Verb("tx", HelpText = "Invokes transactions management module. Please refer to `tx help` output for details")]
internal sealed class TxCommand : CommandBase { }