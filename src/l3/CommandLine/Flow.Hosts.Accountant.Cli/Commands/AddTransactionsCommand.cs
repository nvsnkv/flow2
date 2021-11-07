using System.Diagnostics;
using Flow.Application.Transactions;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Contract;

namespace Flow.Hosts.Accountant.Cli.Commands;

internal class AddTransactionsCommand
{
    private readonly IAccountant accountant;
    private readonly ITransactionsReader reader;
    private readonly IRejectionsWriter writer;
    private readonly IFlowConfiguration config;

    public AddTransactionsCommand(IAccountant accountant, ITransactionsReader reader, IRejectionsWriter writer, IFlowConfiguration config)
    {
        this.accountant = accountant;
        this.reader = reader;
        this.writer = writer;
        this.config = config;
    }

    public async Task Execute(AddTransactionArgs args, CancellationToken ct)
    {
        using var streamReader = CreateReader(args.Input);
        
        var transactions = await reader.ReadTransactions(streamReader, args.Format, ct);
        var rejected = await accountant.Create(transactions, ct);

        var outputPath = args.Output ?? GetFallbackOutputPath(args.Format, "add", "rejected-transactions");
        await using var streamWriter = CreateWriter(outputPath);
        await writer.WriteRejections(streamWriter, rejected, args.Format, ct);

        await TryStartEditor(outputPath, args.Format, false);
    }

    private async Task TryStartEditor(string? outputPath, SupportedFormat format, bool waitForExit)
    {
        if (outputPath != null && (config.Editor?.ContainsKey(format) ?? false))
        {
            var process = Process.Start(config.Editor[format], outputPath);
            if (waitForExit)
            {
                await process.WaitForExitAsync();
            }
        }
    }

    private StreamWriter CreateWriter(string? output)
    {
        var stream = output == null
            ? Console.OpenStandardOutput()
            : File.OpenWrite(output);

        return new StreamWriter(stream);
    }

    private string? GetFallbackOutputPath(SupportedFormat format, string command, string slug)
    {

        if (config.Editor?.ContainsKey(format) ?? false)
        {
            return GeneratePath(format, command, slug);
        }

        return null;
    }

    private string GeneratePath(SupportedFormat format, string command, string slug)
    {
        return $"{command}.{DateTime.Now:s}.{slug}.{format.ToString().ToLower()}";
    }

    private StreamReader CreateReader(string? input)
    {
        var stream = string.IsNullOrEmpty(input)
            ? Console.OpenStandardInput()
            : File.OpenRead(input);

        return new StreamReader(stream);
    }
}