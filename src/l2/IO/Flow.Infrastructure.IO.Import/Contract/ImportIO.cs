using System.Text;
using Autofac;
using Newtonsoft.Json;

namespace Flow.Infrastructure.IO.Import.Contract;

public class ImportIO : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterInstance(new JsonSerializer());
        builder.RegisterType<FolderBasedTransactionsImporter>();
        builder.RegisterType<FileReader>().AsImplementedInterfaces();
        base.Load(builder);

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }
}
