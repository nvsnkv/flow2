using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flow.Application.Transactions.Transfers;
using Flow.Domain.Transactions;
using Flow.Domain.Transactions.Transfers;
using FluentAssertions;
using Moq;
using Xunit;
using Xunit.Categories;

namespace Flow.Application.Transactions.UnitTests;

public class TransferBuilderShould : TestDataCarrier
{
    [Fact] [UnitTest]
    public void BuildTransfersIfDetectorsDetectThem()
    {
        var detector = new Mock<ITransferDetector>();

        var expectedTransfer = SetupDetector(detector, Transactions[1], Transactions[2]);

        var builder = new TransfersBuilder(Transactions.Values).With(detector.Object);
        var result = builder.Build(CancellationToken.None);
        result.Should().BeEquivalentTo(new[] { expectedTransfer });
    }

    [Fact] [UnitTest]
    public void UseAllDetectorsProvided()
    {
        var detector1 = new Mock<ITransferDetector>();
        var detector2 = new Mock<ITransferDetector>();

        var expectedTransfers = new[]
        {
            SetupDetector(detector1, Transactions[1], Transactions[2], "Mock 1"),
            SetupDetector(detector1, Transactions[5], Transactions[4], "Mock 2"),
        };
        
        var builder = new TransfersBuilder(Transactions.Values)
            .With(detector1.Object)
            .With(detector2.Object);

        var result = builder.Build(CancellationToken.None);
        result.Should().BeEquivalentTo(expectedTransfers);
    }

    [Fact] [UnitTest]
    public void NotHaveDuplicatesInSources()
    {
        var detector = new Mock<ITransferDetector>();
        SetupDetector(detector, Transactions[1], Transactions[2]);
        SetupDetector(detector, Transactions[1], Transactions[3]);
        SetupDetector(detector, Transactions[1], Transactions[4]);


        var builder = new TransfersBuilder(Transactions.Values).With(detector.Object);

        var result = (builder.Build(CancellationToken.None)).ToList();

        result.Should().HaveCount(1);
    }

    [Fact] [UnitTest]
    public void NotHaveDuplicatesInSinks()
    {
        var detector = new Mock<ITransferDetector>();
        SetupDetector(detector, Transactions[1], Transactions[2]);
        SetupDetector(detector, Transactions[5], Transactions[2]);
        SetupDetector(detector, Transactions[8], Transactions[2]);
        var builder = new TransfersBuilder(Transactions.Values).With(detector.Object);

        var result = (builder.Build(CancellationToken.None)).ToList();

        result.Should().HaveCount(1);
    }

    private static Transfer SetupDetector(Mock<ITransferDetector> mock, RecordedTransaction left, RecordedTransaction right, string? comment = null)
    {
        var result = new Transfer(left, right) { Comment = comment };

        mock.Setup(d => d.CheckIsTransfer(left, right)).Returns(true);
        mock.Setup(d => d.Create(left, right, CancellationToken.None))
            .Returns(Task.FromResult(result));

        return result;
    }
}