using CommandLine;
using Flow.Hosts.Entrypoint.Commands;

[Verb("bundle", HelpText = "Invokes bundle management module. Please refer to `bundle help` output for details")]
internal sealed class BundleCommand : CommandBase { }