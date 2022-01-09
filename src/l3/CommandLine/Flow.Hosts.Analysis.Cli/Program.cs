using System.Globalization;
using Autofac;
using CommandLine;
using Flow.Application.Analysis.Contract;
using Flow.Application.ExchangeRates.Contract;
using Flow.Application.Transactions.Contract;
using Flow.Hosts.Analysis.Cli.Commands;
using Flow.Hosts.Common;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Contract;
using Flow.Infrastructure.Rates.CBRF.Contract;
using Flow.Infrastructure.Storage.Contract;

var builder = new ContainerBuilder();
builder.RegisterModule(new FlowConfiguration());
var configContainer = builder.Build();

var config = configContainer.Resolve<IFlowConfiguration>();
var culture = CultureInfo.GetCultures(CultureTypes.AllCultures).FirstOrDefault(c => c.Name == config.CultureCode) ?? CultureInfo.CurrentCulture;

builder = new ContainerBuilder();
builder
    .RegisterModule(new FlowConfiguration())
    .RegisterModule(new FlowDatabase())
    .RegisterModule(new FlowIOComponents())
    .RegisterModule(new CBRFData())
    .RegisterModule(new MoneyExchange())
    .RegisterModule(new TransactionsManagement())
    .RegisterModule(new Aggregation(culture));

builder.RegisterType<BuildFlowCommand>();
builder.RegisterType<BuildCalendarCommand>();

var container = builder.Build();

var cancellationHandler = new ConsoleCancellationHandler();
var parser = ParserHelper.Create(culture);

var arguments = parser.ParseArguments<BuildCalendarArgs, BuildFlowArgs>(args);

return await arguments.MapResult(
    async (BuildCalendarArgs arg) => await container.Resolve<BuildCalendarCommand>().Execute(arg, cancellationHandler.Token),
    async (BuildFlowArgs arg) => await container.Resolve<BuildFlowCommand>().Execute(arg, cancellationHandler.Token),
    async errs => await ParserHelper.HandleUnparsed(errs, arguments));