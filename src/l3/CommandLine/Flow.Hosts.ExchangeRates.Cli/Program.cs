using Autofac;
using CommandLine;
using Flow.Application.ExchangeRates.Contract;
using Flow.Hosts.Common;
using Flow.Hosts.ExchangeRates.Cli.Commands;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Contract;
using Flow.Infrastructure.Rates.CBRF.Contract;
using Flow.Infrastructure.Storage.Contract;

var builder = new ContainerBuilder();
builder.RegisterModule(new FlowConfiguration());
var configContainer = builder.Build();

var config = configContainer.Resolve<IFlowConfiguration>();

builder = new ContainerBuilder();

builder.RegisterModule(new FlowConfiguration())
    .RegisterModule(new FlowDatabase())
    .RegisterModule(new FlowIO(config))
    .RegisterModule(new CBRFData())
    .RegisterModule(new MoneyExchange());

builder.RegisterType<ListCommand>();

var container = builder.Build();

var cancellationHandler = new ConsoleCancellationHandler();
var parser = ParserHelper.Create(config);

var arguments = parser.ParseArguments<RequestArgs, ListArgs>(args);
return await arguments.MapResult(
    async (RequestArgs arg) => await container.Resolve<ListCommand>().Execute(arg, cancellationHandler.Token),
    async (ListArgs arg) => await container.Resolve<ListCommand>().Execute(arg, cancellationHandler.Token),
    async errs => await ParserHelper.HandleUnparsed(errs, arguments));
