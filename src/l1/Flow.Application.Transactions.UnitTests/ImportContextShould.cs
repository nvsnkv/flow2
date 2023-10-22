using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Flow.Application.Transactions.Contract;
using Flow.Application.Transactions.Contract.Events;
using Flow.Application.Transactions.Infrastructure;
using Flow.Domain.Transactions;
using FluentAssertions;
using Moq;
using Xunit;
using Xunit.Categories;

namespace Flow.Application.Transactions.UnitTests;

public class ImportContextShould
{
    private readonly TestNotifier notifier = new();

    [Fact, UnitTest]
    public async Task IgnoreTransactionRecordedEvents_AfterDispose()
    {
        var importContext = new ImportContext(notifier, new Mock<IImportContextsFinalizer>().Object, new Mock<IAccountant>().Object);

        await importContext.DisposeAsync();
        notifier.RaiseTransactionRecorded(new(1));

        importContext.RecordedTransactionsCount.Should().Be(0);
    }

    [Fact, UnitTest]
    public async Task ListenTransactionRecordedEvents()
    {
        var importContext = new ImportContext(notifier, new Mock<IImportContextsFinalizer>().Object, new Mock<IAccountant>().Object);

        notifier.RaiseTransactionRecorded(new(1));
        notifier.RaiseTransactionRecorded(new(2));
        notifier.RaiseTransactionRecorded(new(3));

        importContext.RecordedTransactionsCount.Should().Be(3);
    }

    [Fact, UnitTest]
    public async Task CallFinalizerOnComplete()
    {
        var finalizer = new Mock<IImportContextsFinalizer>();
        finalizer.Setup(f => f.Finalize(It.IsAny<IImportContext>(), It.IsAny<CancellationToken>())).Verifiable(Times.Once);

        var importContext = new ImportContext(notifier, finalizer.Object, new Mock<IAccountant>().Object);
        await importContext.Complete(CancellationToken.None);

        finalizer.VerifyAll();
    }

    [Fact, UnitTest]
    public async Task CallFinalizerOnAbort()
    {
        var finalizer = new Mock<IImportContextsFinalizer>();
        finalizer.Setup(f => f.Finalize(It.IsAny<IImportContext>(), It.IsAny<CancellationToken>())).Verifiable(Times.Once);

        var importContext = new ImportContext(notifier, finalizer.Object, new Mock<IAccountant>().Object);
        await importContext.Complete(CancellationToken.None);

        finalizer.VerifyAll();
    }

    [Fact, UnitTest]
    public async Task RemoveTransactionsOnAbort()
    {
        var finalizer = new Mock<IImportContextsFinalizer>();
        var accountant = new Mock<IAccountant>();

        accountant
            .Setup(a => a.DeleteTransactions(It.IsAny<Expression<Func<RecordedTransaction, bool>>>(), It.IsAny<CancellationToken>()))
            .Verifiable(Times.Once);


        var importContext = new ImportContext(notifier, finalizer.Object, accountant.Object);
        await importContext.Abort(CancellationToken.None);

        accountant.VerifyAll();
    }

    [Fact, UnitTest]
    public async Task ProvideSelectionCriteria()
    {
        var finalizer = new Mock<IImportContextsFinalizer>();
        var accountant = new Mock<IAccountant>();

        accountant
            .Setup(a => a.DeleteTransactions(It.IsAny<Expression<Func<RecordedTransaction, bool>>>(), It.IsAny<CancellationToken>()))
            .Verifiable(Times.Once);

        var importContext = new ImportContext(notifier, finalizer.Object, new Mock<IAccountant>().Object);

        var recordedTransactionKey = 50;
        notifier.RaiseTransactionRecorded(new(recordedTransactionKey));
        var expectedTransaction = new RecordedTransaction(recordedTransactionKey, DateTime.Now, 0, string.Empty, null, string.Empty, new(string.Empty, string.Empty));
        var unexpectedTransaction = new RecordedTransaction(61, expectedTransaction);

        var selector = importContext.ImportedTransactionsCriteria.Compile();

        selector(expectedTransaction).Should().BeTrue();
        selector(unexpectedTransaction).Should().BeFalse();
    }

    private sealed class TestNotifier : INotifyTransactionRecorded
    {
        public event TransactionRecordedEventHandler? TransactionRecorded;

        public void RaiseTransactionRecorded(TransactionRecordedEventArgs args)
        {
            TransactionRecorded?.Invoke(this, args);
        }
    }
}
