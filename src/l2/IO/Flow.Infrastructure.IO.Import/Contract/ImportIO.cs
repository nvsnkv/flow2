using Autofac;

namespace Flow.Infrastructure.IO.Import.Contract;

public class ImportIO : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<FolderBasedTransactionsImporter>();
        base.Load(builder);
    }
}
