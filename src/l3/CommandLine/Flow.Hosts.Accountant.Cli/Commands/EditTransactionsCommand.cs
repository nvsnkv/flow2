using Flow.Domain.Transactions;
using Flow.Domain.Transactions.Collections;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Contract;
using System.Linq.Expressions;
using Flow.Application.Transactions.Contract;
using Flow.Domain.Common.Collections;
using Flow.Hosts.Common.Commands;

namespace Flow.Hosts.Accountant.Cli.Commands;

internal class EditTransactionsCommand : CommandBase
{
    private readonly IAccountant accountant;
    private readonly ITransactionsReader reader;
    private readonly ITransactionCriteriaParser criteriaParser;
    private readonly ITransactionsWriter writer;
    private readonly IRejectionsWriter rejectionsWriter;

    public EditTransactionsCommand(IAccountant accountant, ITransactionsReader reader, ITransactionsWriter writer, IRejectionsWriter rejectionsWriter, IFlowConfiguration config, ITransactionCriteriaParser criteriaParser) : base(config)
    {
        this.accountant = accountant;
        this.reader = reader;
        this.rejectionsWriter = rejectionsWriter;
        this.writer = writer;
        this.criteriaParser = criteriaParser;
    }

    public async Task<int> Execute(AddTransactionsArgs args, CancellationToken ct)
    {
        TransactionsWithDateRange<Transaction> initial;
        EnumerableWithCount<RejectedTransaction> rejected;

        using (var streamReader = CreateReader(args.Input))
        {
            initial = new TransactionsWithDateRange<Transaction>(await reader.ReadTransactions(streamReader, args.Format, ct));
            rejected = new EnumerableWithCount<RejectedTransaction>(await accountant.CreateTransactions(initial, ct));
        }

        var errsPath = args.Errors ?? GetFallbackOutputPath(args.Format, "add", "rejected-transactions");
        if (!args.EditInEdior) 
        { 
            await using (var streamWriter = CreateWriter(errsPath))
            {
                await rejectionsWriter.WriteRejections(streamWriter, rejected, args.Format, ct);
            }

            if (rejected.Count > 0)
            {
                return await TryStartEditor(errsPath, args.Format, false);
            }
        }

        if (args.EditInEdior)
        {
            var format = args.Format;
            Expression<Func<RecordedTransaction, bool>> conditions = t => initial.Min <= t.Timestamp && t.Timestamp <= initial.Max;
            var interim = GetFallbackOutputPath(format, "add", "edit-appended");

            return await Edit(conditions, format, ct, errsPath, interim, rejected);
        }

        return 0;
    }

    public async Task<int> Execute(EditTransactionsArgs args, CancellationToken ct)
    {
        var interim = GetFallbackOutputPath(args.Format, "list", "transactions");
        var errors = GetFallbackOutputPath(args.Format, "edit", "rejected-transactions");

        var parserResult = criteriaParser.ParseRecordedTransactionCriteria(args.Criteria ?? Enumerable.Empty<string>());
        if (!parserResult.Successful)
        {
            foreach(var error in parserResult.Errors)
            {
                await Console.Error.WriteLineAsync(error.ToCharArray(), ct);
            }

            return 1;
        }

        return await Edit(parserResult.Conditions, args.Format, ct, interim, errors);
    }

    public async Task<int> Execute(UpdateTransactionsArgs args, CancellationToken ct)
    {
        return await Update(args.Input, args.Format, ct, args.Errors);
    }


    private async Task<int> Edit(Expression<Func<RecordedTransaction, bool>>? conditions, SupportedFormat format, CancellationToken ct, string? interim, string? errsPath, EnumerableWithCount<RejectedTransaction>? rejected = null)
    {
        if (interim == null)
        {
            return -1;
        }

        var appended = await accountant.GetTransactions(conditions, ct);
        await using (var streamWriter = CreateWriter(interim))
        {
            await writer.WriteRecordedTransactions(streamWriter, appended, format, ct);
        }

        var exitCode = await TryStartEditor(interim, format, true);
        if (exitCode != 0)
        {
            return exitCode;
        }

        return await Update(interim, format, ct, errsPath, rejected);
    }

    private async Task<int> Update(string? interim, SupportedFormat format, CancellationToken ct, string? errsPath, EnumerableWithCount<RejectedTransaction>? rejected = null)
    {
        rejected ??= new EnumerableWithCount<RejectedTransaction>(Enumerable.Empty<RejectedTransaction>());

        using var streamReader = CreateReader(interim);

        var updated = await reader.ReadRecordedTransactions(streamReader, format, ct);
        rejected = new EnumerableWithCount<RejectedTransaction>(rejected.Concat(await accountant.UpdateTransactions(updated, ct)));

        await using (var streamWriter = CreateWriter(errsPath))
        {
            await rejectionsWriter.WriteRejections(streamWriter, rejected, format, ct);
        }

        if (rejected.Count > 0)
        {
            return await TryStartEditor(errsPath, format, false);
        }

        return 0;
    }
}