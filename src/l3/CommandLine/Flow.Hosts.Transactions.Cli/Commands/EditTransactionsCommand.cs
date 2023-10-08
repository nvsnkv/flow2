using System.Linq.Expressions;
using Flow.Application.Transactions.Contract;
using Flow.Domain.Common.Collections;
using Flow.Domain.Transactions;
using Flow.Domain.Transactions.Collections;
using Flow.Hosts.Common.Commands;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Transactions.Contract;

namespace Flow.Hosts.Transactions.Cli.Commands;

internal class EditTransactionsCommand : CommandBase
{
    private readonly IAccountant accountant;
    private readonly ISchemaSpecificCollection<ITransactionsReader> readers;
    private readonly ITransactionCriteriaParser criteriaParser;
    private readonly ISchemaSpecificCollection<ITransactionsWriter> writers;
    private readonly IRejectionsWriter rejectionsWriter;

    public EditTransactionsCommand(IAccountant accountant, ISchemaSpecificCollection<ITransactionsReader> readers, ISchemaSpecificCollection<ITransactionsWriter> writers, IRejectionsWriter rejectionsWriter, IFlowConfiguration config, ITransactionCriteriaParser criteriaParser) : base(config)
    {
        this.accountant = accountant;
        this.readers = readers;
        this.rejectionsWriter = rejectionsWriter;
        this.writers = writers;
        this.criteriaParser = criteriaParser;
    }

    public async Task<int> Execute(AddTransactionsArgs args, CancellationToken ct)
    {
        ItemsWithDateRange<(Transaction, Overrides?)> initial;
        EnumerableWithCount<RejectedTransaction> rejected;

        var reader = readers.FindFor(args.Format, args.Schema);
        if (reader == null)
        {
            await Console.Error.WriteLineAsync($"No reader registered for format {args.Format} and schema {args.Schema}");
            return 2;
        }

        using (var streamReader = CreateReader(args.Input))
        {
            initial = new ItemsWithDateRange<(Transaction, Overrides?)>(await reader.ReadTransactions(streamReader, ct), x => x.Item1.Timestamp);
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

            return await Edit(conditions, format, args.Schema, interim, errsPath, ct, rejected);
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

        return await Edit(parserResult.Conditions, args.Format, args.Schema, interim, errors, ct);
    }

    public async Task<int> Execute(UpdateTransactionsArgs args, CancellationToken ct)
    {
        return await Update(args.Input, args.Format, args.Schema, args.Errors, ct);
    }


    private async Task<int> Edit(Expression<Func<RecordedTransaction, bool>>? conditions, SupportedFormat format, SupportedDataSchema schema, string? interim, string? errsPath, CancellationToken ct, EnumerableWithCount<RejectedTransaction>? rejected = null)
    {
        if (interim == null)
        {
            return -1;
        }

        var writer = writers.FindFor(format, schema);
        if (writer == null)
        {
            await Console.Error.WriteLineAsync($"No writer registered for format {format} and schema {schema}");
            return 2;
        }

        var appended = await accountant.GetTransactions(conditions, ct);
        await using (var streamWriter = CreateWriter(interim))
        {
            await writer.WriteRecordedTransactions(streamWriter, appended, ct);
        }

        var exitCode = await TryStartEditor(interim, format, true);
        if (exitCode != 0)
        {
            return exitCode;
        }

        return await Update(interim, format, schema, errsPath, ct, rejected);
    }

    private async Task<int> Update(string? interim, SupportedFormat format, SupportedDataSchema schema, string? errsPath, CancellationToken ct, EnumerableWithCount<RejectedTransaction>? rejected = null)
    {
        rejected ??= new EnumerableWithCount<RejectedTransaction>(Enumerable.Empty<RejectedTransaction>());

        var reader = readers.FindFor(format, schema);
        if (reader == null)
        {
            await Console.Error.WriteLineAsync($"No reader registered for format {format} and schema {schema}");
            return 2;
        }

        using var streamReader = CreateReader(interim);

        var updated = await reader.ReadRecordedTransactions(streamReader, ct);
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
