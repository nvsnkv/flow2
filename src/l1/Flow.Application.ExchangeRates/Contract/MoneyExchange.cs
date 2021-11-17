using System.Runtime.CompilerServices;
using Autofac;
using Flow.Application.ExchangeRates.Validation;

[assembly:InternalsVisibleTo("Flow.Application.ExchangeRates.UnitTests")]

namespace Flow.Application.ExchangeRates.Contract;

public class MoneyExchange : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<ExchangeRateRequestValidator>().AsImplementedInterfaces();
        builder.RegisterType<ExchangeRateValidator>().AsImplementedInterfaces();
        builder.RegisterType<ExchangeRatesProvider>().AsImplementedInterfaces();
        builder.RegisterType<ExchangeRatesManager>().AsImplementedInterfaces();

        base.Load(builder);
    }
}