using System;
using System.Collections.Generic;
using Flow.Application.Transactions.Transfers;
using Flow.Domain.Transactions;
using Flow.Domain.Transactions.Transfers;
using FluentAssertions;
using Moq;
using Xunit;
using Xunit.Categories;

namespace Flow.Application.Transactions.UnitTests;

public class TransferBuilderShould
{
    private static readonly AccountInfo Account = new AccountInfo("name", "bank");

    private static readonly IList<RecordedTransaction> Transactions = new[]
    {
        new RecordedTransaction(1, DateTime.Now, -10, "EUR", "Transfer 1", "Source", Account),
        new RecordedTransaction(2, DateTime.Now, 10, "EUR", "Transfer 1", "Sink", Account),
        new RecordedTransaction(3, DateTime.Now, 10, "EUR", null, "Regular", Account),
        new RecordedTransaction(4, DateTime.Now, 20, "EUR", "Transfer 2", "Sink", Account),
        new RecordedTransaction(5, DateTime.Now.AddMilliseconds(-5), -20, "EUR", "Transfer 2", "Source", Account),
        new RecordedTransaction(6, DateTime.Now, 20, "EUR", null, "Regular 2", Account),
        new RecordedTransaction(6, DateTime.Now, -30, "RUB", null, "Regular 3", Account),
    };

    [Fact] [UnitTest]
    public void BuildTransfersIfDetectorsDetectThem()
    {
        var detector = new Mock<ITransferDetector>();
        detector.Setup(x => x.CheckIsTransfer(Transactions[0], Transactions[1])).Returns(true);

        var expectedTransfer = new Transfer(Transactions[0], Transactions[1]) { Comment = "From mock 1" };
        detector.Setup(x => x.Create(Transactions[0], Transactions[1])).Returns(expectedTransfer);

        var builder = new TransfersBuilder(Transactions).With(detector.Object);
        var result = builder.Build();
        result.Should().BeEquivalentTo(new[] { expectedTransfer });
    }

    [Fact] [UnitTest] 
    public void UseAllDetectorsProvided()
    {
        var expectedTransfers = new[]
        {
            new Transfer(Transactions[0], Transactions[1]) { Comment = "Mock 1" },
            new Transfer(Transactions[4], Transactions[3]) { Comment = "Mock 2" }
        };
        var detector1 = new Mock<ITransferDetector>();
        detector1.Setup(x => x.CheckIsTransfer(Transactions[0], Transactions[1])).Returns(true);
        detector1.Setup(x => x.Create(Transactions[0], Transactions[1])).Returns(expectedTransfers[0]);

        var detector2 = new Mock<ITransferDetector>();
        detector1.Setup(x => x.CheckIsTransfer(Transactions[4], Transactions[3])).Returns(true);
        detector1.Setup(x => x.Create(Transactions[4], Transactions[3])).Returns(expectedTransfers[1]);

        var builder = new TransfersBuilder(Transactions)
            .With(detector1.Object)
            .With(detector2.Object);

        var result = builder.Build();
        result.Should().BeEquivalentTo(expectedTransfers);
    }
}