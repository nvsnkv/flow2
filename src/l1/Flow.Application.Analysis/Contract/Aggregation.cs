using System.Globalization;
using System.Runtime.CompilerServices;
using Autofac;
using Flow.Application.ExchangeRates.Contract;
using Flow.Application.Transactions.Contract;

[assembly:InternalsVisibleTo("Flow.Application.Analysis.UnitTests")]

namespace Flow.Application.Analysis.Contract;

public class Aggregation : Module
{
    private readonly CultureInfo culture;

    public Aggregation(CultureInfo culture)
    {
        this.culture = culture;
    }

    protected override void Load(ContainerBuilder builder)
    {
        builder
            .Register(c => new Aggregator(c.Resolve<IAccountant>(), c.Resolve<IExchangeRatesProvider>(), new Substitutor(culture, new VectorComparer(culture))))
            .AsImplementedInterfaces();

        base.Load(builder);
    }
}