using CommandLine;
using CommandLine.Text;
using Flow.Hosts.Common.Commands;
using Flow.Infrastructure.Configuration.Contract;

namespace Flow.Hosts.Accountant.Cli.Commands.Transfers;

internal class TransfersCommand : CommandBase
{
    private readonly Parser parser = new();

    private readonly ListTransfersCommand listCommand;
    private readonly EditTransfersCommand editCommand;

    public TransfersCommand(IFlowConfiguration config, ListTransfersCommand listCommand, EditTransfersCommand editCommand) : base(config)
    {
        this.listCommand = listCommand;
        this.editCommand = editCommand;
    }

    public async Task<int> Execute(IEnumerable<string> args, CancellationToken ct)
    {
        var arguments = parser.ParseArguments<ListTransfersArgs, EnforceTransfersArgs, AbandonTransfersArgs>(args);
        return await arguments.MapResult(
            async (ListTransfersArgs arg) => await listCommand.Execute(arg, ct),
            async (EnforceTransfersArgs arg) => await editCommand.Execute(arg, ct),
            async (AbandonTransfersArgs arg) => await editCommand.Execute(arg, ct),
            async errs =>
            {
                var width = Console.WindowWidth;
                var errors = errs as Error[] ?? errs.ToArray();
                var output = HelpText.AutoBuild(arguments, h => { h.MaximumDisplayWidth = width; return h; });

                await errors.Output().WriteLineAsync(output);
                return errors.IsHelp() || errors.IsVersion() ? 0 : 1;
            });
    }
}