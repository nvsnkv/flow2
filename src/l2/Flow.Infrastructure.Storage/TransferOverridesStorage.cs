using Flow.Application.Transactions.Infrastructure;
using Flow.Domain.Transactions.Transfers;
using Flow.Infrastructure.Storage.Model;
using Microsoft.EntityFrameworkCore;

namespace Flow.Infrastructure.Storage;

internal class TransferOverridesStorage : ITransferOverridesStorage
{
    private readonly IDbContextFactory<FlowDbContext> factory;

    public TransferOverridesStorage(IDbContextFactory<FlowDbContext> factory)
    {
        this.factory = factory;
    }

    public async Task<IEnumerable<TransferKey>> GetOverrides(CancellationToken ct)
    {
        await using var context = factory.CreateDbContext();
        return await context.EnforcedTransfers.ToListAsync(ct);
    }

    public async Task<IEnumerable<RejectedTransferKey>> Enforce(IEnumerable<TransferKey> keys, CancellationToken ct)
    {
        await using var context = factory.CreateDbContext();
        var rejections = new List<RejectedTransferKey>();

        foreach (var key in keys)
        {
            if (await context.EnforcedTransfers.AnyAsync(t => t.SourceKey == key.SourceKey && t.SinkKey == key.SinkKey, ct))
            {
                rejections.Add(new RejectedTransferKey(key, "Transfer is already enforced!"));
            }
            else if (await context.EnforcedTransfers.AnyAsync(t => t.SourceKey == key.SourceKey ||t.SinkKey == key.SinkKey || t.SourceKey == key.SinkKey || t.SinkKey == key.SourceKey, ct))
            {
                rejections.Add(new RejectedTransferKey(key, "Referenced transaction already used in another enforced transaction!"));
            }
            else if (!await context.Transactions.AnyAsync(t => t.Key == key.SourceKey, ct))
            {
                rejections.Add(new RejectedTransferKey(key, "Unable to find source transaction for this transfer!"));
            }
            else if (!await context.Transactions.AnyAsync(t => t.Key == key.SinkKey, ct))
            {
                rejections.Add(new RejectedTransferKey(key, "Unable to find sink transaction for this transfer!"));
            }
            else
            {
                context.EnforcedTransfers.Add(new DbTransferKey(key));
            }
        }

        await context.SaveChangesAsync(ct);
        return rejections;
    }

    public async Task<IEnumerable<RejectedTransferKey>> Abandon(IEnumerable<TransferKey> keys, CancellationToken ct)
    {
        await using var context = factory.CreateDbContext();
        var rejections = new List<RejectedTransferKey>();

        foreach (var key in keys)
        {
            var target = await context.EnforcedTransfers.FirstOrDefaultAsync(t => t.SourceKey == key.SourceKey && t.SinkKey == key.SinkKey, ct);
            if (target == null)
            {
                rejections.Add(new RejectedTransferKey(key, "Transfer is not enforced"));
            }
            else
            {
                context.EnforcedTransfers.Remove(target);
            }
        }

        await context.SaveChangesAsync(ct);
        return rejections;
    }
}