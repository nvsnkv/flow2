using System.Runtime.CompilerServices;
using Autofac;
using Flow.Infrastructure.IO.Contract;

[assembly: InternalsVisibleTo("Flow.Infrastructure.IO.JSON.UnitTests")]
namespace Flow.Infrastructure.IO.JSON.Contract;

public class JSONIO : Module
{
    public static readonly SupportedFormat SupportedFormat = new("JSON");

    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes().AsImplementedInterfaces();
        builder.RegisterGeneric(typeof(JsonWriter<>)).AsImplementedInterfaces();

        base.Load(builder);
    }
}
