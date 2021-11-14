using System.Threading;
using System.Threading.Tasks;
using Flow.Application.Transactions.Transfers;
using Flow.Domain.Transactions.Transfers;
using FluentAssertions;
using Moq;
using Xunit;
using Xunit.Categories;

namespace Flow.Application.Transactions.UnitTests;

public class TransferBuilderShould : TestDataCarrier
{
    [Fact] [UnitTest]
    public async Task BuildTransfersIfDetectorsDetectThem()
    {
        var detector = new Mock<ITransferDetector>();
        detector.Setup(x => x.CheckIsTransfer(Transactions[1], Transactions[2])).Returns(true);

        var expectedTransfer = new Transfer(Transactions[1], Transactions[2]) { Comment = "From mock 1" };
        detector.Setup(x => x.Create(Transactions[1], Transactions[2], CancellationToken.None)).Returns(Task.FromResult(expectedTransfer));

        var builder = new TransfersBuilder(Transactions.Values).With(detector.Object);
        var result = await builder.Build(CancellationToken.None);
        result.Should().BeEquivalentTo(new[] { expectedTransfer });
    }

    [Fact] [UnitTest] 
    public async Task UseAllDetectorsProvided()
    {
        var expectedTransfers = new[]
        {
            new Transfer(Transactions[1], Transactions[2]) { Comment = "Mock 1" },
            new Transfer(Transactions[5], Transactions[4]) { Comment = "Mock 2" }
        };
        var detector1 = new Mock<ITransferDetector>();
        detector1.Setup(x => x.CheckIsTransfer(Transactions[1], Transactions[2])).Returns(true);
        detector1.Setup(x => x.Create(Transactions[1], Transactions[2], CancellationToken.None)).Returns(Task.FromResult(expectedTransfers[0]));

        var detector2 = new Mock<ITransferDetector>();
        detector1.Setup(x => x.CheckIsTransfer(Transactions[5], Transactions[4])).Returns(true);
        detector1.Setup(x => x.Create(Transactions[5], Transactions[4], CancellationToken.None)).Returns(Task.FromResult(expectedTransfers[1]));

        var builder = new TransfersBuilder(Transactions.Values)
            .With(detector1.Object)
            .With(detector2.Object);

        var result = await builder.Build(CancellationToken.None);
        result.Should().BeEquivalentTo(expectedTransfers);
    }
}