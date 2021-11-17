﻿using Autofac;
using CommandLine;
using Flow.Application.ExchangeRates.Contract;
using Flow.Hosts.Common;
using Flow.Hosts.ExchangeRates.Cli.Commands;
using Flow.Infrastructure.Configuration;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO;
using Flow.Infrastructure.Rates.CBRF.Contract;
using Flow.Infrastructure.Storage;

var builder = new ContainerBuilder();

builder.RegisterModule(new FlowConfiguration())
    .RegisterModule(new FlowDatabase())
    .RegisterModule(new FlowIOComponents())
    .RegisterModule(new CBRFData())
    .RegisterModule(new MoneyExchange());

builder.RegisterType<ListCommand>();

var container = builder.Build();
var config = container.Resolve<IFlowConfiguration>();

var parser = ParserHelper.Create(config);

var arguments = parser.ParseArguments<RequestArgs, ListArgs>(args);
return await arguments.MapResult(
    async (RequestArgs arg) => await container.Resolve<ListCommand>().Execute(arg, CancellationToken.None),
    async (ListArgs arg) => await container.Resolve<ListCommand>().Execute(arg, CancellationToken.None),
    async errs => await ParserHelper.HandleUnparsed(errs, arguments));

