using System.Text;
using Autofac;

namespace Flow.Infrastructure.IO.Import.Contract;

public class ImportIO : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<FolderBasedTransactionsImporter>();
        builder.RegisterType<FileReader>().AsImplementedInterfaces();
        base.Load(builder);

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }
}
