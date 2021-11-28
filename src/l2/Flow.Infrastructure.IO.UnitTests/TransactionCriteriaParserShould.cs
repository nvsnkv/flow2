using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Flow.Domain.Transactions;
using Flow.Infrastructure.IO.Criteria;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

namespace Flow.Infrastructure.IO.UnitTests;

public class TransactionCriteriaParserShould : TestDataCarrier
{
    [Theory, UnitTest]
    [MemberData(nameof(TransactionParameters))]
    public void CreateTransactionCriteria(string cultureCode, string textInput, Func<Transaction, bool> expectedSelector)
    {
        var culture = CultureInfo.GetCultureInfo(cultureCode);
        var input = textInput.Split(' ');
        var result = new TransactionCriteriaParser(new GenericParser(culture, DateTimeStyles.AssumeLocal, NumberStyles.Any)).ParseRecordedTransactionCriteria(input);
        result.Successful.Should().BeTrue();
        result.Errors.Should().BeEmpty();

        var items = RecordedTransactions.Where(result.Conditions!.Compile()).ToList();

        items.Should().BeEquivalentTo(Transactions.Where(expectedSelector).ToList());
    }

    [Theory, UnitTest]
    [MemberData(nameof(RecordedTransactionParameters))]
    public void CreateRecordedTransactionCriteria(string cultureCode, string textInput, Func<RecordedTransaction, bool> expectedSelector)
    {
        var culture = CultureInfo.GetCultureInfo(cultureCode);
        var input = textInput.Split(' ');
        var result = new TransactionCriteriaParser(new GenericParser(culture, DateTimeStyles.AssumeLocal, NumberStyles.Any)).ParseRecordedTransactionCriteria(input);
        result.Successful.Should().BeTrue();
        result.Errors.Should().BeEmpty();

        var items = RecordedTransactions.Where(result.Conditions!.Compile()).ToList();

        items.Should().BeEquivalentTo(RecordedTransactions.Where(expectedSelector).ToList());
    }


    public static IEnumerable<object[]> TransactionParameters => new []
    {
        new object[] {"a<=-150", (Func<Transaction, bool>)(a => a.Amount <= -150) },
        new object[] {"a<=-150", (Func<Transaction, bool>)(a => a.Amount <= -150) },
        new object[] { "!a>0 c=RUB t%Кирил", (Func<Transaction, bool>)(a => !(a.Amount > 0) && a.Currency == "RUB" && a.Title.Contains("Кирил")) },

        new object[] {"ts[2021-11-05:2021-11-06)", (Func<Transaction, bool>)(a => new DateTime(2021, 11, 05).ToUniversalTime() <= a.Timestamp &&  a.Timestamp < new DateTime(2021, 11, 06).ToUniversalTime()) },
        new object[] {"ts(2021-11-05:2021-11-06]", (Func<Transaction, bool>)(a => new DateTime(2021, 11, 05).ToUniversalTime() < a.Timestamp &&  a.Timestamp <= new DateTime(2021, 11, 06).ToUniversalTime()) },
        new object[] {"ts[2021-11-05:2021-11-06]", (Func<Transaction, bool>)(a => new DateTime(2021, 11, 05).ToUniversalTime() <= a.Timestamp &&  a.Timestamp <= new DateTime(2021, 11, 06).ToUniversalTime()) },
        new object[] {"ts[2021-11-05:2021-11-06)", (Func<Transaction, bool>)(a => new DateTime(2021, 11, 05).ToUniversalTime() <= a.Timestamp &&  a.Timestamp < new DateTime(2021, 11, 06).ToUniversalTime()) },
        new object[] {"ts(2021-11-05:2021-11-06]", (Func<Transaction, bool>)(a => new DateTime(2021, 11, 05).ToUniversalTime() < a.Timestamp &&  a.Timestamp <= new DateTime(2021, 11, 06).ToUniversalTime()) },
        new object[] {"ts[2021-11-05:2021-11-06]", (Func<Transaction, bool>)(a => new DateTime(2021, 11, 05).ToUniversalTime() <= a.Timestamp &&  a.Timestamp <= new DateTime(2021, 11, 06).ToUniversalTime()) },

        new object[] {"c(RUB,EUR) t%s", (Func<Transaction, bool>)(a => new [] {"RUB", "EUR" }.Contains(a.Currency) && a.Title.Contains('s')), },
        new object[] {"c(RUB,EUR) t%s", (Func<Transaction, bool>)(a => new [] {"RUB", "EUR" }.Contains(a.Currency) && a.Title.Contains('s')), }
    }.Join(
        new[]{ "ru-RU", "en-US" }, 
        _ => true, _ => true, 
        (a, c) => Enumerable.Repeat((object)c, 1).Concat(a).ToArray()
        )
        .Concat(new[]
        {
            new object[] {"ru-RU", "a<=-150,5", (Func<Transaction, bool>)(a => a.Amount <= -150.5M) },
            new object[] { "ru-RU", "a<=-150,5", (Func<Transaction, bool>)(a => a.Amount <= -150.5M) },

            new object[] {"en-US", "a<=-150.5", (Func<Transaction, bool>)(a => a.Amount <= -150.5M) },
            new object[] { "en-US", "a<=-150.5", (Func<Transaction, bool>)(a => a.Amount <= -150.5M) },
        });

    public static IEnumerable<object[]> RecordedTransactionParameters => new[]
    {
        new object[] {"a<=-150", (Func<RecordedTransaction, bool>)(a => a.Amount <= -150) },
        new object[] {"a<=-150", (Func<RecordedTransaction, bool>)(a => a.Amount <= -150) },
        new object[] { "!a>0 c=RUB t%Кирил", (Func<RecordedTransaction, bool>)(a => !(a.Amount > 0) && a.Currency == "RUB" && a.Title.Contains("Кирил")) },

        new object[] {"ts[2021-11-05:2021-11-06)", (Func<RecordedTransaction, bool>)(a => new DateTime(2021, 11, 05).ToUniversalTime() <= a.Timestamp &&  a.Timestamp < new DateTime(2021, 11, 06).ToUniversalTime()) },
        
        new object[] {"c(RUB,EUR) t%s", (Func<RecordedTransaction, bool>)(a => new [] {"RUB", "EUR" }.Contains(a.Currency) && a.Title.Contains('s')), }
    }.Join(
        new[] { "ru-RU", "en-US" },
        _ => true, _ => true,
        (a, c) => Enumerable.Repeat((object)c, 1).Concat(a).ToArray()
        )
        .Concat(new[]
        {
            new object[] {"ru-RU", "k(3,5)", (Func<RecordedTransaction, bool>)(a => new long[]{ 3, 5 }.Contains(a.Key)) },
            new object[] {"ru-RU", "ocom=noway ocat%2 ot%1", (Func<RecordedTransaction, bool>)(a => a.Overrides?.Comment == "noway" && (a.Overrides?.Category?.Contains('2') ?? false) && (a.Overrides?.Title?.Contains('2') ?? false)), }
        });
}