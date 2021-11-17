using Flow.Application.Transactions.Contract;
using Flow.Domain.Common.Collections;
using Flow.Domain.Transactions.Transfers;
using Flow.Hosts.Common.Commands;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Contract;
using JetBrains.Annotations;

namespace Flow.Hosts.Transfers.Cli.Commands;

[UsedImplicitly]
internal class EditTransfersCommand : CommandBase
{
    private readonly ITransferKeysReader reader;
    private readonly IRejectionsWriter writer;
    private readonly IAccountant accountant;

    public EditTransfersCommand(IFlowConfiguration config, IAccountant accountant, IRejectionsWriter writer, ITransferKeysReader reader) : base(config)
    {
        this.accountant = accountant;
        this.writer = writer;
        this.reader = reader;
    }

    public async Task<int> Execute(EnforceTransfersArgs args, CancellationToken ct)
    {
        EnumerableWithCount<RejectedTransferKey> rejections;

        using (var streamReader = CreateReader(args.Input))
        {
            var keys = await reader.ReadTransferKeys(streamReader, args.Format, ct);
            rejections = new EnumerableWithCount<RejectedTransferKey>(await accountant.EnforceTransfers(keys, ct));
        }

        var errsPath = args.Errors ?? GetFallbackOutputPath(args.Format, "enforce", "rejected-transfers");
        await using (var streamWriter = CreateWriter(args.Errors))
        {
            await writer.WriteRejections(streamWriter, rejections, args.Format, ct);
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
            var keys = await reader.ReadTransferKeys(streamReader, args.Format, ct);
            rejections = new EnumerableWithCount<RejectedTransferKey>(await accountant.AbandonTransfers(keys, ct));
        }

        var errsPath = args.Errors ?? GetFallbackOutputPath(args.Format, "abandon", "rejected-transfers");
        await using (var streamWriter = CreateWriter(args.Errors))
        {
            await writer.WriteRejections(streamWriter, rejections, args.Format, ct);
        }

        if (rejections.Count > 0)
        {
            return await TryStartEditor(errsPath, args.Format, false);
        }

        return 0;
    }
}