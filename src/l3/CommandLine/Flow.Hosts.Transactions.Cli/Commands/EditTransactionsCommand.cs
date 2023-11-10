using System.Linq.Expressions;
using Flow.Application.Transactions.Contract;
using Flow.Domain.Common.Collections;
using Flow.Domain.Transactions;
using Flow.Domain.Transactions.Collections;
using Flow.Hosts.Common.Commands;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Collections;
using Flow.Infrastructure.IO.Contract;
using Flow.Infrastructure.IO.Criteria.Contract;
using Flow.Infrastructure.IO.CSV.Contract;

namespace Flow.Hosts.Transactions.Cli.Commands;

internal class EditTransactionsCommand : CommandBase
{
    private readonly IAccountant accountant;
    private readonly ITransactionCriteriaParser criteriaParser;
    private readonly IReaders<IncomingTransaction> transactionReaders;
    private readonly IReaders<RecordedTransaction> recordedTransactionReaders;
    private readonly IWriters<RecordedTransaction> recorderTransactionWriters;
    private readonly IWriters<RejectedTransaction> rejectionsWriter;

    public EditTransactionsCommand(IAccountant accountant, IFlowConfiguration config, ITransactionCriteriaParser criteriaParser, IReaders<IncomingTransaction> transactionReaders, IReaders<RecordedTransaction> recordedTransactionReaders, IWriters<RecordedTransaction> recorderTransactionWriters, IWriters<RejectedTransaction> rejectionsWriter) : base(config)
    {
        this.accountant = accountant;
        this.criteriaParser = criteriaParser;
        this.transactionReaders = transactionReaders;
        this.recordedTransactionReaders = recordedTransactionReaders;
        this.recorderTransactionWriters = recorderTransactionWriters;
        this.rejectionsWriter = rejectionsWriter;
    }

    public async Task<int> Execute(AddTransactionsArgs args, CancellationToken ct)
    {
        ItemsWithDateRange<IncomingTransaction> initial;
        EnumerableWithCount<RejectedTransaction> rejected;

        var reader = transactionReaders.GetFor(args.Format);


        using (var streamReader = CreateReader(args.Input))
        {
            initial = new ItemsWithDateRange<IncomingTransaction>(await reader.Read(streamReader, ct), x => x.Transaction.Timestamp);
            rejected = new EnumerableWithCount<RejectedTransaction>(await accountant.CreateTransactions(initial, ct));
        }

        var errsPath = args.Errors ?? GetFallbackOutputPath(args.Format, "add", "rejected-transactions");
        if (!args.EditInEditor)
        { 
            await using (var streamWriter = CreateWriter(errsPath))
            {
                var format = rejectionsWriter.GetKnownFormats().Contains(args.Format) ? args.Format : CSVIO.SupportedFormat;
                await rejectionsWriter.GetFor(format).Write(streamWriter, rejected, ct);
            }

            if (rejected.Count > 0)
            {
                return await TryStartEditor(errsPath, args.Format, false);
            }
        }

        if (args.EditInEditor)
        {
            var format = args.Format;
            Expression<Func<RecordedTransaction, bool>> conditions = t => initial.Min <= t.Timestamp && t.Timestamp <= initial.Max;
            var interim = GetFallbackOutputPath(format, "add", "edit-appended");

            return await Edit(conditions, format, interim, errsPath, ct, rejected);
        }

        return 0;
    }

    public async Task<int> Execute(EditTransactionsArgs args, CancellationToken ct)
    {
        var interim = GetFallbackOutputPath(args.Format, "list", "transactions");
        var errors = args.Output ?? GetFallbackOutputPath(args.Format, "edit", "rejected-transactions");

        var parserResult = criteriaParser.ParseRecordedTransactionCriteria(args.Criteria ?? Enumerable.Empty<string>());
        if (!parserResult.Successful)
        {
            foreach(var error in parserResult.Errors)
            {
                await Console.Error.WriteLineAsync(error.ToCharArray(), ct);
            }

            return 1;
        }

        return await Edit(parserResult.Conditions, args.Format, interim, errors, ct);
    }

    public async Task<int> Execute(UpdateTransactionsArgs args, CancellationToken ct)
    {
        return await Update(args.Input, args.Format, args.Errors, ct);
    }


    private async Task<int> Edit(Expression<Func<RecordedTransaction, bool>>? conditions, SupportedFormat format, string? interim, string? errsPath, CancellationToken ct, EnumerableWithCount<RejectedTransaction>? rejected = null)
    {
        if (interim == null)
        {
            return -1;
        }

        var writer = recorderTransactionWriters.GetFor(format);

        var appended = await accountant.GetTransactions(conditions, ct);
        await using (var streamWriter = CreateWriter(interim))
        {
            await writer.Write(streamWriter, appended, ct);
        }

        var exitCode = await TryStartEditor(interim, format, true);
        if (exitCode != 0)
        {
            return exitCode;
        }

        return await Update(interim, format, errsPath, ct, rejected);
    }

    private async Task<int> Update(string? interim, SupportedFormat format, string? errsPath, CancellationToken ct, EnumerableWithCount<RejectedTransaction>? rejected = null)
    {
        rejected ??= new EnumerableWithCount<RejectedTransaction>(Enumerable.Empty<RejectedTransaction>());

        var reader = recordedTransactionReaders.GetFor(format);

        using var streamReader = CreateReader(interim);

        var updated = await reader.Read(streamReader, ct);
        rejected = new EnumerableWithCount<RejectedTransaction>(rejected.Concat(await accountant.UpdateTransactions(updated, ct)));

        await using (var streamWriter = CreateWriter(errsPath))
        {
            await rejectionsWriter.GetFor(format).Write(streamWriter, rejected, ct);
        }

        if (rejected.Count > 0)
        {
            return await TryStartEditor(errsPath, format, false);
        }

        return 0;
    }
}
