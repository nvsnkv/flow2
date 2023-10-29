using System.Threading.Tasks;
using Flow.Application.Transactions.Contract;
using Flow.Application.Transactions.Contract.Events;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

namespace Flow.Application.Transactions.UnitTests;

public class ImportContextShould
{
    private readonly TestNotifier notifier = new();

    [Fact, UnitTest]
    public async Task IgnoreTransactionRecordedEvents_AfterDispose()
    {
        var importContext = new ImportContext(notifier);

        await importContext.DisposeAsync();
        notifier.RaiseTransactionRecorded(new(1));

        importContext.RecordedTransactionsCount.Should().Be(0);
    }

    [Fact, UnitTest]
    public void ListenTransactionRecordedEvents()
    {
        var importContext = new ImportContext(notifier);

        notifier.RaiseTransactionRecorded(new(1));
        notifier.RaiseTransactionRecorded(new(2));
        notifier.RaiseTransactionRecorded(new(3));

        importContext.RecordedTransactionsCount.Should().Be(3);
    }


    [Fact, UnitTest]
    public void ProvideRecordedTransactionKeys()
    {
        var expectedKeys = new long[] { 50, 52, 33, 1 };

        var importContext = new ImportContext(notifier);

        foreach (var key in expectedKeys)
        {
            notifier.RaiseTransactionRecorded(new(key));
        }

        importContext.RecordedTransactionsCount.Should().Be(expectedKeys.Length);
        importContext.RecordedTransactionKeys.Should().BeEquivalentTo(expectedKeys);
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
