using Autofac;

namespace Flow.Infrastructure.Rates.CBRF.Contract;

public class CBRFData : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<CBRFRemoteRatesProvider>().AsImplementedInterfaces();

        base.Load(builder);
    }
}