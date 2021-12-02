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

builder.RegisterModule(new FlowConfiguration())
    .RegisterModule(new FlowDatabase())
    .RegisterModule(new FlowIOComponents())
    .RegisterModule(new CBRFData())
    .RegisterModule(new MoneyExchange())
    .RegisterModule(new TransactionsManagement())
    .RegisterModule(new Aggregation());

builder.RegisterType<BuildFlowCommand>();
builder.RegisterType<BuildCalendarCommand>();

var container = builder.Build();

var config = container.Resolve<IFlowConfiguration>();

var parser = ParserHelper.Create(config);

var arguments = parser.ParseArguments<BuildFlowArgs, BuildCalendarArgs>(args);

return await arguments.MapResult(
    async (BuildFlowArgs arg) => await container.Resolve<BuildFlowCommand>().Execute(arg, CancellationToken.None),
    async (BuildCalendarArgs arg) => await container.Resolve<BuildCalendarCommand>().Execute(arg, CancellationToken.None),
    async errs => await ParserHelper.HandleUnparsed(errs, arguments));