using Autofac;
using CommandLine;
using CommandLine.Text;
using Flow.Hosts.Accountant.Cli.Commands;
using Flow.Infrastructure.Configuration;
using Flow.Infrastructure.IO;
using Flow.Infrastructure.Storage;

var builder = new ContainerBuilder();
    builder.RegisterModule(new FlowConfiguration())
    .RegisterModule(new FlowDatabase())
    .RegisterModule(new FlowIOComponents());

var container = builder.Build();


var arguments = new Parser().ParseArguments<AddTransactionArgs>(args.Take(1));
await arguments.MapResult(
    async arg => await container.Resolve<AddTransactionsCommand>().Execute(arg, CancellationToken.None),
    async errs =>
    {
        var errors = errs as Error[] ?? errs.ToArray();
        var output = errors.IsHelp() || errors.IsVersion()
            ? HelpText.AutoBuild(arguments, h => h)
            : HelpText.AutoBuild(arguments, h => HelpText.DefaultParsingErrorsHandler(arguments, h));

        await errors.Output().WriteLineAsync(output);
    });