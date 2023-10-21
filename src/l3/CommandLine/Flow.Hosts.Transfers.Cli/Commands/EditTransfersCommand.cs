using Flow.Application.Transactions.Contract;
using Flow.Domain.Common.Collections;
using Flow.Domain.Transactions.Transfers;
using Flow.Hosts.Common.Commands;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Collections;
using JetBrains.Annotations;

namespace Flow.Hosts.Transfers.Cli.Commands;

[UsedImplicitly]
internal class EditTransfersCommand : CommandBase
{
    private readonly IReaders<TransferKey> reader;
    private readonly IWriters<RejectedTransferKey> writer;
    private readonly IAccountant accountant;

    public EditTransfersCommand(IFlowConfiguration config, IAccountant accountant, IReaders<TransferKey> reader, IWriters<RejectedTransferKey> writer) : base(config)
    {
        this.accountant = accountant;
        this.reader = reader;
        this.writer = writer;
    }

    public async Task<int> Execute(EnforceTransfersArgs args, CancellationToken ct)
    {
        EnumerableWithCount<RejectedTransferKey> rejections;

        using (var streamReader = CreateReader(args.Input))
        {
            var keys = await reader.GetFor(args.Format).Read(streamReader, ct);
            rejections = new EnumerableWithCount<RejectedTransferKey>(await accountant.EnforceTransfers(keys, ct));
        }

        var errsPath = args.Errors ?? GetFallbackOutputPath(args.Format, "enforce", "rejected-transfers");
        await using (var streamWriter = CreateWriter(args.Errors))
        {
            await writer.GetFor(args.Format).Write(streamWriter, rejections, ct);
        }

        if (rejections.Count > 0)
        {
            return await TryStartEditor(errsPath, args.Format, false);
        }

        return 0;
    }

    public async Task<int> Execute(AbandonTransfersArgs args, CancellationToken ct)
    {
        EnumerableWithCount<RejectedTransferKey> rejections;

        using (var streamReader = CreateReader(args.Input))
        {
            var keys = await reader.GetFor(args.Format).Read(streamReader, ct);
            rejections = new EnumerableWithCount<RejectedTransferKey>(await accountant.AbandonTransfers(keys, ct));
        }

        var errsPath = args.Errors ?? GetFallbackOutputPath(args.Format, "abandon", "rejected-transfers");
        await using (var streamWriter = CreateWriter(args.Errors))
        {
            await writer.GetFor(args.Format).Write(streamWriter, rejections, ct);
        }

        if (rejections.Count > 0)
        {
            return await TryStartEditor(errsPath, args.Format, false);
        }

        return 0;
    }
}
