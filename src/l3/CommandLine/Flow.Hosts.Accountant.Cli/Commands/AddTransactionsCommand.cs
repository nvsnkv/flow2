﻿using Flow.Application.Transactions;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Contract;

namespace Flow.Hosts.Accountant.Cli.Commands;

internal class AddTransactionsCommand : CommandBase
{
    private readonly IAccountant accountant;
    private readonly ITransactionsReader reader;
    private readonly IRejectionsWriter writer;

    public AddTransactionsCommand(IAccountant accountant, ITransactionsReader reader, IRejectionsWriter writer, IFlowConfiguration config) : base(config)
    {
        this.accountant = accountant;
        this.reader = reader;
        this.writer = writer;
    }

    public async Task<int> Execute(AddTransactionsArgs addTransactionsArgs, CancellationToken ct)
    {
        using var streamReader = CreateReader(addTransactionsArgs.Input);
        
        var transactions = await reader.ReadRecordedTransactions(streamReader, addTransactionsArgs.Format, ct);
        var rejected = await accountant.Update(transactions, ct);

        var outputPath = addTransactionsArgs.Output ?? GetFallbackOutputPath(addTransactionsArgs.Format, "add", "rejected-transactions");
        await using var streamWriter = CreateWriter(outputPath);
        await writer.WriteRejections(streamWriter, rejected, addTransactionsArgs.Format, ct);

        return await TryStartEditor(outputPath, addTransactionsArgs.Format, false);
    }
}