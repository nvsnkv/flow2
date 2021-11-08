using System.Globalization;
using Autofac;
using CommandLine;
using CommandLine.Text;
using Flow.Application.Transactions.Contract;
using Flow.Hosts.Accountant.Cli.Commands;
using Flow.Infrastructure.Configuration;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO;
using Flow.Infrastructure.Storage;

var builder = new ContainerBuilder();

builder.RegisterModule(new FlowConfiguration())
    .RegisterModule(new FlowDatabase())
    .RegisterModule(new FlowIOComponents())
    .RegisterModule(new TransactionsManagement());

builder.RegisterType<AddTransactionsCommand>();
builder.RegisterType<ListTransactionsCommand>();

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

var arguments = parser.ParseArguments<AddTransactionsArgs, ListTransactionsArgs>(args);
return await arguments.MapResult(
    async (AddTransactionsArgs arg) => await container.Resolve<AddTransactionsCommand>().Execute(arg, CancellationToken.None),
    async (ListTransactionsArgs arg) => await container.Resolve<ListTransactionsCommand>().Execute(arg, CancellationToken.None),
    async errs =>
    {
        var errors = errs as Error[] ?? errs.ToArray();
        var output = errors.IsHelp() || errors.IsVersion()
            ? HelpText.AutoBuild(arguments, h => h)
            : HelpText.AutoBuild(arguments, h => HelpText.DefaultParsingErrorsHandler(arguments, h));

        await errors.Output().WriteLineAsync(output);
        return errors.IsHelp() || errors.IsVersion() ? 0 : 1;
    });