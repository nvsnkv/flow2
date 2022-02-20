using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Flow.Domain.Transactions;
using Flow.Infrastructure.IO.Criteria;
using Flow.Infrastructure.IO.Dimensions;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

namespace Flow.Infrastructure.IO.UnitTests;

public class AggregationSetupParserShould
{
    private readonly AccountInfo accountInfo = new("account", "bank");
    
    private readonly string simpleSetup = @" Category; Title
                                             Total; Expense;a<0
                                             Total; Income;a>0";

    private readonly string invalidSetup = @"Dimension 1; Dimension 2
                                             More; Values; Than; Measure; ...
                                             Less; Values
                                             Bad; Criteria; ...
                                             
                                             Valid; But ignored;a=0";

    private readonly string multipleConditions = @"Dminesion 1
                                                   group 1; a<0
                                                   group 1; t=1
                                                   group 2; a>=5";

    private readonly string cascading = @"Dimension
                                          group: totals
                                          total;k>=0
                                          group: A
                                          As;t%A
                                          subgroup: b
                                          Bs;t%b
                                          Cs;t%c";

    private readonly AggregationSetupParser parser;

    public AggregationSetupParserShould()
    {
        parser = new AggregationSetupParser(';', new TransactionCriteriaParser(new GenericParser(CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, NumberStyles.Any)));
    }

    [Fact, IntegrationTest]
    public async Task ParseSimpleSetup()
    {
        await using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(simpleSetup));
        using var reader = new StreamReader(memoryStream);
        var result = await parser.ParseFromStream(reader, CancellationToken.None);

        result.Successful.Should().BeTrue();
        result.Errors.Should().BeEmpty();

        var setup = result.Setup!;
        setup.Should().NotBeNull();
        setup.Dimensions.Should().BeEquivalentTo("Category", "Title");
        setup.Sections.Count.Should().Be(1);

        var group = setup.Sections.Single();
        group.Name.Should().NotBeNullOrEmpty();

        group.Rules.Should().HaveCount(2);
        
        var rule = group.Rules[0];
        rule.Measure.Should().BeEquivalentTo("Total", "Expense");

        rule.Rule(new RecordedTransaction(0, DateTime.UtcNow, -1, "RUR", null, "Expense", accountInfo)).Should().BeTrue();
        rule.Rule(new RecordedTransaction(0, DateTime.UtcNow, 1, "RUR", null, "Income", accountInfo)).Should().BeFalse();

        rule = group.Rules[1];
        rule.Measure.Should().BeEquivalentTo("Total", "Income");
        rule.Rule(new RecordedTransaction(0, DateTime.UtcNow, -1, "RUR", null, "Expense", accountInfo)).Should().BeFalse();
        rule.Rule(new RecordedTransaction(0, DateTime.UtcNow, 1, "RUR", null, "Income", accountInfo)).Should().BeTrue();

        group.Alternative.Should().BeNull();
    }

    [Fact, IntegrationTest]
    public async Task ReportErrors()
    {
        await using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(invalidSetup));
        using var reader = new StreamReader(memoryStream);
        var result = await parser.ParseFromStream(reader, CancellationToken.None);

        result.Successful.Should().BeFalse();
        result.Setup.Should().BeNull();

        var expectedErrors = new[]
        {
            "2: Failed to parse aggregation rule! Rule must contain have same amount of dimensions as defined in header and must ends with transaction criteria!",
            "3: Failed to parse aggregation rule! Rule must contain have same amount of dimensions as defined in header and must ends with transaction criteria!",
            $"4: Failed to parse criteria for [Bad, Criteria]: {Environment.NewLine}  Unable to parse criterion: ...",
            "5: Empty string detected!"
        };

        result.Errors.Should().BeEquivalentTo(expectedErrors);
    }

    [Fact, IntegrationTest]
    public async Task CombineDiffererntRules() 
    {
        await using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(multipleConditions));
        using var reader = new StreamReader(memoryStream);
        var result = await parser.ParseFromStream(reader, CancellationToken.None);

        result.Successful.Should().BeTrue();
        result.Setup.Should().NotBeNull();
        result.Setup!.Sections.Should().HaveCount(1);
        var group = result.Setup.Sections.Single();

        group.Rules.Should().HaveCount(2);
        var group1 = group.Rules[0];
        var group2 = group.Rules[1];

        var negativeGroup1 = new RecordedTransaction(0, DateTime.UtcNow, -1, "RUR", null, "title", accountInfo);
        var titleGroup1 = new RecordedTransaction(0, DateTime.UtcNow, 2, "RUR", null, "1", accountInfo);
        var amountGroup2 = new RecordedTransaction(0, DateTime.UtcNow, 15, "RUR", null, "title", accountInfo);   

        group1.Rule(negativeGroup1).Should().BeTrue();
        group1.Rule(titleGroup1).Should().BeTrue();
        group1.Rule(amountGroup2).Should().BeFalse();

        group2.Rule(negativeGroup1).Should().BeFalse();
        group2.Rule(titleGroup1).Should().BeFalse();
        group2.Rule(amountGroup2).Should().BeTrue();
    }

    [Fact, IntegrationTest]
    public async Task BuildCascadedRules() 
    {
        await using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(cascading));
        using var reader = new StreamReader(memoryStream);
        var result = await parser.ParseFromStream(reader, CancellationToken.None);

        result.Successful.Should().BeTrue();
        result.Setup.Should().NotBeNull();

        var groups = result.Setup!.Sections;
        groups.Should().HaveCount(2);

        var t = groups[0];
        t.Name.Should().Be("totals");
        t.Rules.Should().HaveCount(1);
        t.Rules.Single().Measure.Should().BeEquivalentTo("total");

        var a = groups[1];
        a.Name.Should().Be("A");
        a.Rules.Should().HaveCount(1);
        a.Rules.Single().Measure.Should().BeEquivalentTo("As");
        a.Alternative.Should().NotBeNull();
        
        var b = a.Alternative!;
        b.Name.Should().Be("b");
        b.Rules.Should().HaveCount(2);
        b.Rules[0].Measure.Should().BeEquivalentTo("Bs");
        b.Rules[1].Measure.Should().BeEquivalentTo("Cs");
        b.Alternative.Should().BeNull();
    }
}