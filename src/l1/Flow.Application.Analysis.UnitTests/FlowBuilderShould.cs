using Flow.Domain.Analysis;
using Flow.Domain.Transactions;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

namespace Flow.Application.Analysis.UnitTests;

public class FlowBuilderShould
{
    private static readonly AccountInfo Account = new("Account", "Bank");

    private static readonly RecordedTransaction[] Expenses =
    {
        new(1, DateTime.UtcNow, -100, "RUR", null, "Expense 1", Account),
        new(2, DateTime.UtcNow, -101, "RUR", null, "Expense 2", Account),
        new(3, DateTime.UtcNow, -102, "RUR", null, "Expense 3", Account)
    };

    private static readonly RecordedTransaction[] Incomes =
    {
        new(11, DateTime.UtcNow, 100, "RUR", null, "Income 1", Account),
        new(12, DateTime.UtcNow, 101, "RUR", null, "Income 2", Account),
        new(13, DateTime.UtcNow, 102, "RUR", null, "Income 3", Account),
        new(14, DateTime.UtcNow, 102, "RUR", null, "Income 4", Account)
    };

    [Fact, UnitTest]
    public async Task GenerateExpensesAndIncomes()
    {
        var builder = new FlowBuilder(Expenses.Concat(Incomes));
        var flow = await builder.Build(CancellationToken.None).ToListAsync();

        flow.Should().HaveCount(Expenses.Length + Incomes.Length);
        flow.Where(f => f is Income).Should().BeEquivalentTo(Incomes.Select(i => new Income(i)));
        flow.Where(f => f is Expense).Should().BeEquivalentTo(Expenses.Select(i => new Expense(i)));
    }
}
