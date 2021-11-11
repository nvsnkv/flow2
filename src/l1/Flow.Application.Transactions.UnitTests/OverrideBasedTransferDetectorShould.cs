using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Flow.Application.Transactions.Infrastructure;
using Flow.Application.Transactions.Transfers;
using Flow.Domain.Transactions;
using Flow.Domain.Transactions.Transfers;
using FluentAssertions;
using Moq;
using Xunit;
using Xunit.Categories;

namespace Flow.Application.Transactions.UnitTests;

public class OverrideBasedTransferDetectorShould :TestDataCarrier
{
    private static readonly IEnumerable<TransferKey> Enforced = new[] { new TransferKey(1, 2), new TransferKey(400, 344) };
    private readonly Mock<ITransferOverridesStorage> storage = new();

    public OverrideBasedTransferDetectorShould()
    {
        storage.Setup(s => s.GetOverrides(It.IsAny<CancellationToken>())).Returns(Task.FromResult(Enforced));
    }

    [Fact] [UnitTest]
    public async Task DetectEnforcedTransfers()
    {
        var detector = await OverridesBasedTransferDetector.Create(storage.Object, CancellationToken.None);
        detector.CheckIsTransfer(Transactions[1], Transactions[2]).Should().BeTrue();
    }

    [Fact] [UnitTest]
    public async Task CreateEnforcedTransfers()
    {
        var expectedTransfer = new Transfer(Transactions[400], Transactions[344]) {Comment = "User defined transfer" };
        var detector = await OverridesBasedTransferDetector.Create(storage.Object, CancellationToken.None);
        var transfer = detector.Create(Transactions[400], Transactions[344]);

        transfer.Should().Be(expectedTransfer);
        transfer.Comment.Should().Be(expectedTransfer.Comment);
    }

    [Fact]
    [UnitTest]
    public async Task IgnoreNotEnforcedTransfers()
    {
        var detector = await OverridesBasedTransferDetector.Create(storage.Object, CancellationToken.None);
        detector.CheckIsTransfer(Transactions[5], Transactions[4]).Should().BeFalse();
    }
}