using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Flow.Application.ExchangeRates.Infrastructure;
using Flow.Domain.ExchangeRates;
using FluentAssertions;
using Moq;
using Xunit;
using Xunit.Categories;

namespace Flow.Application.ExchangeRates.UnitTests;

public class ExchangeRatesProviderShould
{
    private readonly ExchangeRate[] storedRates =
    {
        new("RUR", "MNT", DateTime.Parse("2021-11-13"), 0.03M),
        new("MNT", "RUR", DateTime.Parse("2021-11-13"), 33.3333333M)
    };

    private readonly ExchangeRate remoteRate = new("EUR", "USD", DateTime.Parse("2021-11-13"), 1.15M);

    private readonly Mock<IExchangeRatesStorage> storageMock = new();
    private readonly Mock<IRemoteExchangeRatesProvider> remoteMock = new();
    private readonly ExchangeRatesProvider provider;

    public ExchangeRatesProviderShould()
    {
        storageMock
            .Setup(s => s.Read(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult((IEnumerable<ExchangeRate>)storedRates))
            .Verifiable();

        storageMock
            .Setup(s => s.Create(It.IsAny<ExchangeRate>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        remoteMock
            .Setup(r => r.GetRate(It.IsAny<ExchangeRateRequest>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(remoteRate)!)
            .Verifiable();

        provider = new ExchangeRatesProvider(storageMock.Object, remoteMock.Object);
    }

    [Fact] [UnitTest]
    public async Task ReturnRateLoadedFromStorage()
    {
        var rate = await provider.GetRate(new ExchangeRateRequest("RUR", "MNT", DateTime.Parse("2021-11-13")), CancellationToken.None);

        rate.Should().Be(storedRates[0]);
        remoteMock
            .Verify(
                r => r.GetRate(It.IsAny<ExchangeRateRequest>(), It.IsAny<CancellationToken>()),
                Times.Never
                );
    }

    [Fact] [UnitTest]
    public void ReadRatesFromStorageOnce()
    {
        var tasks = new Task[]
        {
            provider.GetRate(new ExchangeRateRequest("RUR", "MNT", DateTime.Parse("2021-11-13")), CancellationToken.None),
            provider.GetRate(new ExchangeRateRequest("MNT", "RUR", DateTime.Parse("2021-11-13")), CancellationToken.None),
        };

        Task.WaitAll(tasks);

        storageMock.Verify(s => s.Read(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact] [UnitTest]
    public async Task RequestRemoteRateWhenNothingFoundLocally()
    {
        var rate = await provider.GetRate(new ExchangeRateRequest("EUR", "USD", DateTime.Parse("2021-11-13")), CancellationToken.None);

        rate.Should().Be(remoteRate);
        remoteMock.Verify(
            r => r.GetRate(It.IsAny<ExchangeRateRequest>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact] [UnitTest]
    public async Task SaveRemoteRateToLocalStorage()
    {
        var rate = await provider.GetRate(new ExchangeRateRequest("EUR", "USD", DateTime.Parse("2021-11-13")), CancellationToken.None);
        await Task.Delay(300);

        rate.Should().Be(remoteRate);
        storageMock.Verify(s => s.Create(It.IsAny<ExchangeRate>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}