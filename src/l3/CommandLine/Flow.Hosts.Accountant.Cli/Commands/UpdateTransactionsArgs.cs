using System.ComponentModel.DataAnnotations;
using CommandLine;

namespace Flow.Hosts.Accountant.Cli.Commands;

[Verb("update", HelpText = "Update transactions that already recorded.")]
internal class UpdateTransactionsArgs : AddTransactionsArgs
{
}