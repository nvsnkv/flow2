using Flow.Application.Transactions.Contract;
using Flow.Application.Transactions.Contract.Events;
using Flow.Application.Transactions.Infrastructure;
using Newtonsoft.Json;

namespace Flow.Infrastructure.IO.Import.Contract;

public sealed class FolderBasedTransactionsImporter : ITransactionImporter
{
    private static readonly string ContextFileName = ".flow-import-context";

    private readonly JsonSerializer serializer;
    private readonly INotifyTransactionRecorded notifier;
    private string workspace = Environment.CurrentDirectory;


    public FolderBasedTransactionsImporter(JsonSerializer serializer, INotifyTransactionRecorded notifier)
    {
        this.serializer = serializer;
        this.notifier = notifier;
    }

    public string Workspace
    {
        get => workspace;
        set
        {
            if (!Directory.Exists(workspace)) throw new ArgumentException("Workspace must be an existing directory!", nameof(value));
            workspace = value;
        }
    }

    public async Task<IImportContext> GetContext(CancellationToken ct)
    {
        var items = await LoadContext();
        return new ImportContext(items, notifier);

    }

    public Task Save(IImportContext context, CancellationToken ct)
    {
        using var writer = new JsonTextWriter(new StreamWriter(File.OpenWrite(ContextFilePath)));
        serializer.Serialize(writer, context.RecordedTransactionKeys);

        File.SetAttributes(ContextFilePath, FileAttributes.Hidden);

        return Task.CompletedTask;
    }

    public Task Complete(IImportContext context, CancellationToken ct)
    {
        if (File.Exists(ContextFilePath))
        {
            File.Delete(ContextFilePath);
        }

        return Task.CompletedTask;
    }

    private string ContextFilePath => Path.Combine(Workspace, ContextFileName);

    private async Task<IEnumerable<long>> LoadContext()
    {
        if (!File.Exists(ContextFilePath))
        {
            return Enumerable.Empty<long>();
        }

        await using var reader = new JsonTextReader(new StreamReader(File.OpenRead(ContextFilePath)));
        return serializer.Deserialize<IEnumerable<long>>(reader) ?? Enumerable.Empty<long>();
    }
}
