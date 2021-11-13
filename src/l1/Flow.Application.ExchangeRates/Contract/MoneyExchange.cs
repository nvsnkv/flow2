using System.Runtime.CompilerServices;
using Autofac;

[assembly:InternalsVisibleTo("FLow.Application.ExchangeRates.UnitTests")]

namespace Flow.Application.ExchangeRates.Contract;

public class MoneyExchange : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<ExchangeRatesProvider>().AsImplementedInterfaces();
        base.Load(builder);
    }
}