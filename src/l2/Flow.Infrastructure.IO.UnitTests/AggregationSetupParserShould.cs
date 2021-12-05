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
                                             More; Values; Than; Dimensions; ...
                                             Less; Values
                                             Bad; Criteria; ...
                                             
                                             Valid; But ignored;a=0";

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
        setup.Headers.Should().BeEquivalentTo("Category", "Title");
        setup.Groups.Count.Should().Be(1);

        var group = setup.Groups.Single();
        group.Name.Should().NotBeNullOrEmpty();

        group.Rules.Should().HaveCount(2);
        
        var rule = group.Rules[0];
        rule.Dimensions.Should().BeEquivalentTo("Total", "Expense");

        rule.Rule(new RecordedTransaction(0, DateTime.UtcNow, -1, "RUR", null, "Expense", accountInfo)).Should().BeTrue();
        rule.Rule(new RecordedTransaction(0, DateTime.UtcNow, 1, "RUR", null, "Income", accountInfo)).Should().BeFalse();

        rule = group.Rules[1];
        rule.Dimensions.Should().BeEquivalentTo("Total", "Income");
        rule.Rule(new RecordedTransaction(0, DateTime.UtcNow, -1, "RUR", null, "Expense", accountInfo)).Should().BeFalse();
        rule.Rule(new RecordedTransaction(0, DateTime.UtcNow, 1, "RUR", null, "Income", accountInfo)).Should().BeTrue();

        group.Subgroup.Should().BeNull();
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
}