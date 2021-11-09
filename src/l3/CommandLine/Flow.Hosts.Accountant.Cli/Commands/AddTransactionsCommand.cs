using Flow.Application.Transactions;
using Flow.Domain.Transactions;
using Flow.Domain.Transactions.Collections;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Contract;

namespace Flow.Hosts.Accountant.Cli.Commands;

internal class AddTransactionsCommand : CommandBase
{
    private readonly IAccountant accountant;
    private readonly ITransactionsReader reader;
    private readonly ITransactionsWriter writer;
    private readonly IRejectionsWriter rejectionsWriter;

    public AddTransactionsCommand(IAccountant accountant, ITransactionsReader reader, ITransactionsWriter writer, IRejectionsWriter rejectionsWriter, IFlowConfiguration config) : base(config)
    {
        this.accountant = accountant;
        this.reader = reader;
        this.rejectionsWriter = rejectionsWriter;
        this.writer = writer;
    }

    public async Task<int> Execute(AddTransactionsArgs args, CancellationToken ct)
    {
        TransactionsWithDateRange<Transaction> initial;
        EnumerableWithCount<RejectedTransaction> rejected;

        using (var streamReader = CreateReader(args.Input))
        {
            initial = new TransactionsWithDateRange<Transaction>(await reader.ReadTransactions(streamReader, args.Format, ct));
            rejected = new EnumerableWithCount<RejectedTransaction>(await accountant.Create(initial, ct));
        }

        var errsPath = args.Errors ?? GetFallbackOutputPath(args.Format, "add", "rejected-transactions");
        await using (var streamWriter = CreateWriter(errsPath))
        {
            await rejectionsWriter.WriteRejections(streamWriter, rejected, args.Format, ct);
        }

        if (rejected.Count > 0 && !args.Interactive)
        {
            return await TryStartEditor(errsPath, args.Format, false);
        }

        if (args.Interactive)
        {
            var interimFile = GetFallbackOutputPath(args.Format, "add", "edit-appended");
            if (interimFile == null)
            {
                return -1;
            }

            var appended = await accountant.Get(t => initial.Min <= t.Timestamp && t.Timestamp <= initial.Max, ct);
            await using (var streamWriter = CreateWriter(interimFile))
            {
                await writer.WriteRecordedTransactions(streamWriter, appended, args.Format, ct);
            }

            var exitCode = await TryStartEditor(interimFile, args.Format, true);
            if (exitCode != 0)
            {
                return exitCode;
            }

            using (var streamReader = new StreamReader(interimFile))
            {
                var updated = await reader.ReadRecordedTransactions(streamReader, args.Format, ct);
                rejected = new EnumerableWithCount<RejectedTransaction>(rejected.Concat(await accountant.Update(updated, ct)));

                await using (var streamWriter = CreateWriter(errsPath))
                {
                    await rejectionsWriter.WriteRejections(streamWriter, rejected, args.Format, ct);
                }

                if (rejected.Count > 0)
                {
                    return await TryStartEditor(errsPath, args.Format, false);
                }
            }
        }

        return 0;
    }
}