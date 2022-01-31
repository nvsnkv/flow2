using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Flow.Application.ExchangeRates.Contract;
using Flow.Application.Transactions.Infrastructure;
using Flow.Application.Transactions.Transfers;
using Flow.Domain.ExchangeRates;
using Flow.Domain.Transactions.Transfers;
using FluentAssertions;
using Moq;
using Xunit;
using Xunit.Categories;

namespace Flow.Application.Transactions.UnitTests;

public class OverrideBasedTransferDetectorShould :TestDataCarrier
{
    private static readonly IEnumerable<TransferKey> Enforced = new[] { new TransferKey(1, 2), new TransferKey(400, 344), new TransferKey(500, 501) };
    private readonly Mock<ITransferOverridesStorage> storage = new();
    private readonly Mock<IExchangeRatesProvider> ratesProvider = new();

    public OverrideBasedTransferDetectorShould()
    {
        storage.Setup(s => s.GetOverrides(It.IsAny<CancellationToken>())).Returns(Task.FromResult(Enforced));
    }

    [Fact, UnitTest]
    public async Task DetectEnforcedTransfers()
    {
        var detector = await OverridesBasedTransferDetector.Create(storage.Object, ratesProvider.Object, CancellationToken.None);
        detector.CheckIsTransfer(Transactions[1], Transactions[2]).Should().BeTrue();
    }

    [Fact, UnitTest]
    public async Task SetAccuracyLevelToExact()
    {
        var detector = await OverridesBasedTransferDetector.Create(storage.Object, ratesProvider.Object, CancellationToken.None);
        var result = await detector.Create(Transactions[1], Transactions[2], CancellationToken.None);
        result.AccuracyLevel.Should().Be(DetectionAccuracy.Exact);
    }

    [Fact, UnitTest]
    public async Task CreateEnforcedTransfers()
    {
        var expectedTransfer = new Transfer(Transactions[400], Transactions[344], DetectionAccuracy.Exact) {Comment = "User defined transfer" };
        var detector = await OverridesBasedTransferDetector.Create(storage.Object, ratesProvider.Object, CancellationToken.None);
        var transfer = await detector.Create(Transactions[400], Transactions[344], CancellationToken.None);

        transfer.Should().Be(expectedTransfer);
        transfer.Comment.Should().Be(expectedTransfer.Comment);
    }

    [Fact, UnitTest]
    public async Task IgnoreNotEnforcedTransfers()
    {
        var detector = await OverridesBasedTransferDetector.Create(storage.Object, ratesProvider.Object, CancellationToken.None);
        detector.CheckIsTransfer(Transactions[5], Transactions[4]).Should().BeFalse();
    }

    [Fact, UnitTest]
    public async Task ApplyConversionRatesWhenCurrenciesDiffer()
    {
        const decimal rate = 0.034M;
        ratesProvider
            .Setup(s => s.GetRate(It.IsAny<ExchangeRateRequest>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new ExchangeRate("RUB", "BYN", DateTime.Now, rate))!);

        var detector = await OverridesBasedTransferDetector.Create(storage.Object, ratesProvider.Object, CancellationToken.None);
        var transfer = await detector.Create(Transactions[500], Transactions[501], CancellationToken.None);

        transfer.Fee.Should().Be(Transactions[500].Amount + Transactions[501].Amount * rate);
        transfer.Currency.Should().Be(Transactions[500].Currency);
    }
}