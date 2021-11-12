using System.Globalization;
using Autofac;
using CommandLine;
using CommandLine.Text;
using Flow.Application.Transactions.Contract;
using Flow.Hosts.Accountant.Cli.Commands;
using Flow.Hosts.Accountant.Cli.Commands.Transfers;
using Flow.Infrastructure.Configuration;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO;
using Flow.Infrastructure.Storage;

var builder = new ContainerBuilder();

builder.RegisterModule(new FlowConfiguration())
    .RegisterModule(new FlowDatabase())
    .RegisterModule(new FlowIOComponents())
    .RegisterModule(new TransactionsManagement());

builder.RegisterType<EditTransactionsCommand>();
builder.RegisterType<ListTransactionsCommand>();
builder.RegisterType<DeleteTransactionsCommand>();

builder.RegisterType<TransfersCommand>();
builder.RegisterType<ListTransfersCommand>();
builder.RegisterType<EditTransfersCommand>();

var container = builder.Build();
var config = container.Resolve<IFlowConfiguration>();

var parser = new Parser(settings => { 
    settings.AutoHelp = true;
    settings.AutoVersion = true;
    settings.CaseInsensitiveEnumValues = true;
    settings.IgnoreUnknownArguments = true;
    settings.ParsingCulture =
        CultureInfo.GetCultures(CultureTypes.AllCultures)
            .FirstOrDefault(c => c.Name == config.CultureCode) 
        ?? CultureInfo.CurrentCulture;

    settings.EnableDashDash = true;
});

var arguments = parser.ParseArguments<AddTransactionsArgs, ListTransactionsArgs, UpdateTransactionsArgs, EditTransactionsArgs, DeleteTransactionsArgs, TransfersArgs>(args);
return await arguments.MapResult(
    async (AddTransactionsArgs arg) => await container.Resolve<EditTransactionsCommand>().Execute(arg, CancellationToken.None),
    async (ListTransactionsArgs arg) => await container.Resolve<ListTransactionsCommand>().Execute(arg, CancellationToken.None),
    async (UpdateTransactionsArgs arg) => await container.Resolve<EditTransactionsCommand>().Execute(arg, CancellationToken.None),
    async (EditTransactionsArgs arg) => await container.Resolve<EditTransactionsCommand>().Execute(arg, CancellationToken.None),
    async (DeleteTransactionsArgs arg) => await container.Resolve<DeleteTransactionsCommand>().Execute(arg, CancellationToken.None),
    async (TransfersArgs _) => await container.Resolve<TransfersCommand>().Execute(args.Skip(1), CancellationToken.None),
    async errs =>
    {
        var width = Console.WindowWidth;
        var errors = errs as Error[] ?? errs.ToArray();
        var output = HelpText.AutoBuild(arguments, h => { h.MaximumDisplayWidth = width; return h; });

        await errors.Output().WriteLineAsync(output);
        return errors.IsHelp() || errors.IsVersion() ? 0 : 1;
    });