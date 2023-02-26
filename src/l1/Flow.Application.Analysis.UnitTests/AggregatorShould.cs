using System.Globalization;
using System.Linq.Expressions;
using Flow.Domain.Transactions;
using Flow.Application.ExchangeRates.Contract;
using Flow.Application.Transactions.Contract;
using Moq;
using Xunit;
using Xunit.Categories;
using FluentAssertions;

namespace Flow.Application.Analysis.UnitTests;

public sealed class AggregatorShould
{
    private static readonly AccountInfo Account = new("Account", "Bank");

    private static readonly RecordedTransaction[] Transactions =
    {
        new(11, DateTime.UtcNow, 100, "RUR", null, "Income 1", Account),
        new(12, DateTime.UtcNow, 101, "RUR", null, "Income 2", Account),
        new(13, DateTime.UtcNow, 102, "RUR", null, "Income 3", Account),
        new(14, DateTime.UtcNow, 102, "RUR", null, "Income 4", Account)
    };

    private readonly Mock<IAccountant> accountant = new();
    private readonly Mock<IExchangeRatesProvider> ratesProvider = new();

    private readonly Aggregator aggregator;
    public AggregatorShould()
    {
        accountant
            .Setup(a => a.GetTransactions(It.IsAny<Expression<Func<RecordedTransaction, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Transactions);

        var culture = CultureInfo.GetCultureInfo("ru-RU");
        aggregator = new Aggregator(accountant.Object, ratesProvider.Object, new Substitutor(culture), new SeriesBuilderComparer(new VectorComparer(culture)));
    }

    /// <summary>
    /// Definitely not the best test I wrote. TODO redesign it, add proper unit tests for <see cref="Substitutor"/>, <see cref="SeriesBuilderComparer"/> and <see cref="Aggregator"/> itself.
    /// Alternatively, create an integration test that uses mocked L2 objects only
    /// </summary>
    /// <returns></returns>
    [Fact, IntegrationTest]
    public async Task FilterOutTransaction_ThatDoesNotMatchCriteria()
    {
        Expression<Func<RecordedTransaction, bool>> criteria = t => t.Key != 13;

        var (flow, _) = await aggregator.GetFlow(new(DateTime.MinValue, DateTime.MaxValue, "RUR", criteria));

        flow.Should().NotBeNull();
        var results = await flow.ToListAsync();

        results.Should().BeEquivalentTo(Transactions.Where(criteria.Compile()));
    }
}