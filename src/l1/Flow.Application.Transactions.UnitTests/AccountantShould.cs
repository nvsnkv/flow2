﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flow.Application.ExchangeRates.Contract;
using Flow.Application.Transactions.Contract;
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
    private static readonly RecordedTransaction[] RecordedTransactions =
    {
        new(100, DateTime.UtcNow, -10, "RUB", null, "Transaction 100", new("Account", "Bank"), Guid.NewGuid().ToString()),
        new(101, DateTime.UtcNow, 11, "RUB", null, "Transaction 101", new("Account", "Bank"), Guid.NewGuid().ToString()),
        new(102, DateTime.UtcNow, -12, "RUB", null, "Transaction 102", new("Account", "Bank"), Guid.NewGuid().ToString()),
        new(103, DateTime.UtcNow, 13, "RUB", null, "Transaction 103", new("Account", "Bank"), Guid.NewGuid().ToString()),
        new(104, DateTime.UtcNow, 104, "RUB", null, "Transaction 104", new("Account", "Bank"), Guid.NewGuid().ToString()),
        new(105, DateTime.UtcNow, -104, "RUB", null, "Transaction 105", new("Account", "Bank"), Guid.NewGuid().ToString()),
        new(106, DateTime.UtcNow, 104, "RUB", null, "Transaction 106", new("Account", "Bank"), Guid.NewGuid().ToString())
    };

    private readonly Mock<ITransactionsStorage> storage = new();
    private readonly Mock<IValidator<Transaction>> transactionValidator = new();
    private readonly Mock<ITransferOverridesStorage> overridesStorage = new();
    private readonly Mock<IValidator<TransferKey>> transferKeyValidator = new();
    private readonly Mock<IExchangeRatesProvider> ratesProvider = new();
    private readonly TestTransferDetector[] detectors;

    private Accountant accountant;

    public AccountantShould()
    {
        storage
            .Setup(s => s.Read(Constants<RecordedTransaction>.Truth, CancellationToken.None))
            .Returns(Task.FromResult(RecordedTransactions as IEnumerable<RecordedTransaction>));

        transactionValidator.Setup(v => v.Validate(It.IsAny<Transaction>())).Returns(new ValidationResult());

        detectors = new[]
        {
            new TestTransferDetector(new (long, long)[] { (100, 101) }, DetectionAccuracy.Exact),
            new TestTransferDetector(new (long, long)[] { (102, 104) }, DetectionAccuracy.Exact),
            new TestTransferDetector(new (long, long)[] { (102, 103) }, DetectionAccuracy.Likely),
            new TestTransferDetector(new (long, long)[] { (105, 106) }, DetectionAccuracy.Likely)
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
    public async Task GetExactTransfersOnlyOnGetTransfersInvoked()
    {
        var exactTransfers = await accountant.GetTransfers(Constants<RecordedTransaction>.Truth, CancellationToken.None)
            .ToListAsync(CancellationToken.None);
        exactTransfers.Count.Should().Be(2);

        var first = exactTransfers.First();
        first.Source.Should().BeEquivalentTo(RecordedTransactions.First(s => s.Key == 100));
        first.Sink.Should().BeEquivalentTo(RecordedTransactions.First(s => s.Key == 101));

        var second = exactTransfers.Last();
        second.Source.Should().BeEquivalentTo(RecordedTransactions.First(s => s.Key == 102));
        second.Sink.Should().BeEquivalentTo(RecordedTransactions.First(s => s.Key == 104));
    }

    [Fact, UnitTest]
    //See transfer detector setup in ctor
    public async Task ReturnPossibleTransfersWithoutExactOnes()
    {
        var likelyTransfers = await accountant
            .GuessTransfers(Constants<RecordedTransaction>.Truth, CancellationToken.None)
            .ToListAsync(CancellationToken.None);
        likelyTransfers.Count.Should().Be(1);
        var t = likelyTransfers.Single();
        t.Source.Should().BeEquivalentTo(RecordedTransactions.First(s => s.Key == 105));
        t.Sink.Should().BeEquivalentTo(RecordedTransactions.First(s => s.Key == 106));
    }

    [Theory, UnitTest]
    [MemberData(nameof(TransactionsWithDuplicates))]
    public async Task GuessDuplicates(string testCase, RecordedTransaction[] input, RecordedTransaction[][] expectedResults)
    {
        testCase.Should().NotBeEmpty();

        storage
            .Setup(s => s.Read(Constants<RecordedTransaction>.Truth, CancellationToken.None))
            .Returns(Task.FromResult(input as IEnumerable<RecordedTransaction>));

        accountant = new Accountant(
            storage.Object,
            transactionValidator.Object,
            detectors,
            overridesStorage.Object,
            transferKeyValidator.Object,
            ratesProvider.Object
        );

        var result = (await accountant.GuessDuplicates(null, 3, CancellationToken.None)).ToList();
        result.Should().BeEquivalentTo(expectedResults);
    }

    public static readonly IEnumerable<object[]> TransactionsWithDuplicates = new[]
    {
        new object[]
        {
            "Exact duplicates",
            new RecordedTransaction[]
            {
                new(5, DateTime.Parse("2022-01-01"), 100, "RUB", null, "Transaction 1", new("Account", "Bank"), Guid.NewGuid().ToString()),
                new(7, DateTime.Parse("2022-01-01"), 100, "RUB", null, "Transaction 1", new("Account", "Bank"), Guid.NewGuid().ToString()),
                new(8, DateTime.Parse("2022-01-01"), 100, "RUB", null, "Transaction 1", new("Account", "Bank"), Guid.NewGuid().ToString()),
                    new(0, DateTime.Parse("2022-01-01"), 100, "RUB", null, "Not a dup", new("Account", "Bank"), Guid.NewGuid().ToString()),
            },
            new[]
            {
                new RecordedTransaction[]
                {
                    new(5, DateTime.Parse("2022-01-01"), 100, "RUB", null, "Transaction 1", new("Account", "Bank"), Guid.NewGuid().ToString()),
                    new(7, DateTime.Parse("2022-01-01"), 100, "RUB", null, "Transaction 1", new("Account", "Bank"), Guid.NewGuid().ToString()),
                    new(8, DateTime.Parse("2022-01-01"), 100, "RUB", null, "Transaction 1", new("Account", "Bank"), Guid.NewGuid().ToString()),
                }
            }
        },

        new object[]
        {
            "Different dates",
            new RecordedTransaction[]
            {
                new(5, DateTime.Parse("2022-01-01"), 100, "RUB", null, "Transaction 1", new("Account", "Bank"), Guid.NewGuid().ToString()),
                new(7, DateTime.Parse("2022-01-02"), 100, "RUB", null, "Transaction 1", new("Account", "Bank"), Guid.NewGuid().ToString()),
                new(8, DateTime.Parse("2022-01-03"), 100, "RUB", null, "Transaction 1", new("Account", "Bank"), Guid.NewGuid().ToString()),
                new(0, DateTime.Parse("2022-01-01"), 100, "RUB", null, "Not a dup", new("Account", "Bank"), Guid.NewGuid().ToString()),
            },
            new[]
            {
                new RecordedTransaction[]
                {
                    new(5, DateTime.Parse("2022-01-01"), 100, "RUB", null, "Transaction 1", new("Account", "Bank"), Guid.NewGuid().ToString()),
                    new(7, DateTime.Parse("2022-01-02"), 100, "RUB", null, "Transaction 1", new("Account", "Bank"), Guid.NewGuid().ToString()),
                    new(8, DateTime.Parse("2022-01-03"), 100, "RUB", null, "Transaction 1", new("Account", "Bank"), Guid.NewGuid().ToString()),
                }
            }
        },

        new object[]
        {
            "Two sets",
            new RecordedTransaction[]
            {
                new(5, DateTime.Parse("2022-01-01"), 100, "RUB", null, "Transaction 2", new("Account", "Bank"), Guid.NewGuid().ToString()),
                new(7, DateTime.Parse("2022-01-02"), 100, "RUB", null, "Transaction 1", new("Account", "Bank"), Guid.NewGuid().ToString()),
                new(8, DateTime.Parse("2022-01-03"), 100, "RUB", null, "Transaction 1", new("Account", "Bank"), Guid.NewGuid().ToString()),
                new(0, DateTime.Parse("2022-01-01"), 100, "RUB", null, "Not a dup", new("Account", "Bank"), Guid.NewGuid().ToString()),
                new(9, DateTime.Parse("2022-01-01"), 100, "RUB", null, "Transaction 2", new("Account", "Bank"), Guid.NewGuid().ToString()),
            },
            new[]
            {
                new RecordedTransaction[]
                {
                    new(7, DateTime.Parse("2022-01-02"), 100, "RUB", null, "Transaction 1", new("Account", "Bank"), Guid.NewGuid().ToString()),
                    new(8, DateTime.Parse("2022-01-03"), 100, "RUB", null, "Transaction 1", new("Account", "Bank"), Guid.NewGuid().ToString()),
                },
                new RecordedTransaction[]
                {
                    new(5, DateTime.Parse("2022-01-01"), 100, "RUB", null, "Transaction 2", new("Account", "Bank"), Guid.NewGuid().ToString()),
                    new(9, DateTime.Parse("2022-01-01"), 100, "RUB", null, "Transaction 2", new("Account", "Bank"), Guid.NewGuid().ToString()),
                }
            }
        },

        new object[]
        {
            "No dups",
            new RecordedTransaction[]
            {
                new(5, DateTime.Parse("2022-01-01"), 110, "RUB", null, "Transaction 1", new("Account", "Bank"), Guid.NewGuid().ToString()),
                new(7, DateTime.Parse("2022-01-01"), 100, "RUB", null, "Transaction 1", new("Account", "Bank"), Guid.NewGuid().ToString()),
                new(8, DateTime.Parse("2022-01-01"), 102, "RUB", null, "Transaction 1", new("Account", "Bank"), Guid.NewGuid().ToString()),
                new(0, DateTime.Parse("2022-01-01"), 100, "RUB", null, "Not a dup", new("Account", "Bank"), Guid.NewGuid().ToString()),
            },
            Array.Empty<RecordedTransaction[]>()
        }
    };

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
