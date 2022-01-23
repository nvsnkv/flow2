using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flow.Application.ExchangeRates.Contract;
using Flow.Application.Transactions.Infrastructure;
using Flow.Application.Transactions.Transfers;
using Flow.Domain.Patterns;
using Flow.Domain.Transactions;
using Flow.Domain.Transactions.Transfers;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Xunit;
using Xunit.Categories;

namespace Flow.Application.Transactions.UnitTests;

public class AccountantShould
{
    private static readonly RecordedTransaction[] RecordedTransactions = {
        new(100, DateTime.UtcNow, -10, "RUB", null, "Transaction 100", new("Account", "Bank")),
        new(101, DateTime.UtcNow, 11, "RUB", null, "Transaction 101", new("Account", "Bank")),
        new(102, DateTime.UtcNow, -12, "RUB", null, "Transaction 102", new("Account", "Bank")),
        new(103, DateTime.UtcNow, 13, "RUB", null, "Transaction 103", new("Account", "Bank")),
        new(104, DateTime.UtcNow, -104, "RUB", null, "Transaction 104", new("Account", "Bank")),
    };

    private readonly Mock<ITransactionsStorage> storage = new();
    private readonly Mock<IValidator<Transaction>> transactionValidator = new();
    private readonly Mock<ITransferOverridesStorage> overridesStorage = new();
    private readonly Mock<IValidator<TransferKey>> transferKeyValidator = new();
    private readonly Mock<IExchangeRatesProvider> ratesProvider = new();

    private readonly Accountant accountant;

    public AccountantShould()
    {
        storage
            .Setup(s => s.Read(Constants<RecordedTransaction>.Truth, CancellationToken.None))
            .Returns(Task.FromResult(RecordedTransactions as IEnumerable<RecordedTransaction>));

        transactionValidator.Setup(v => v.Validate(It.IsAny<Transaction>())).Returns(new ValidationResult());

        var detectors = new[] { 
            new TestTransferDetector(new (long, long)[] { (100, 101) }, DetectionAccuracy.Exact),
            new TestTransferDetector(new (long, long)[] { (102, 103) }, DetectionAccuracy.Likely)
        };

        accountant = new Accountant(
            storage.Object,
            transactionValidator.Object,
            detectors,
            overridesStorage.Object,
            transferKeyValidator.Object,
            ratesProvider.Object
        );
    }

    [Fact, UnitTest]
    public async Task ReadTransactionsFromTheStorage()
    {
        var result = await accountant.GetTransactions(null, CancellationToken.None);
        result.ToList().Should().BeEquivalentTo(RecordedTransactions);
    }

    [Fact, UnitTest]
    //See transfer detector setup in ctor
    public async Task GetExactTransfersWhenJustExactAccuracyRequested()
    {
        var exactTransfers = await accountant.GetTransfers(Constants<RecordedTransaction>.Truth, DetectionAccuracy.Exact, CancellationToken.None).ToListAsync(CancellationToken.None);
        exactTransfers.Count.Should().Be(1);
        var t = exactTransfers.Single();
        t.Source.Should().Be(100);
        t.Sink.Should().Be(101);
    }

    [Fact, UnitTest]
    public async Task GetOnlyLikelyTransfersWhenLikelyTransfersWhenLikelyAccuracyRequested()
    {
        var transfers = await accountant.GetTransfers(Constants<RecordedTransaction>.Truth, DetectionAccuracy.Likely, CancellationToken.None).ToListAsync(CancellationToken.None);
        transfers.Count.Should().Be(1);

        var likely = transfers.Single();
        likely.Source.Should().Be(102);
        likely.Sink.Should().Be(103);
    }

    private class TestTransferDetector : ITransferDetector
    {
        private readonly (long, long)[] transfers;

        public TestTransferDetector((long, long)[] transfers, DetectionAccuracy accuracy)
        {
            Accuracy = accuracy;
            this.transfers = transfers;
        }

        public bool CheckIsTransfer(RecordedTransaction left, RecordedTransaction right)
        {
            return transfers.Any(t => t.Item1 == left.Key && t.Item2 == right.Key);
        }

        public Task<Transfer> Create(RecordedTransaction left, RecordedTransaction right, CancellationToken ct)
        {
            return Task.FromResult(new Transfer(left, right, Accuracy));
        }

        public DetectionAccuracy Accuracy { get; }
    }
}