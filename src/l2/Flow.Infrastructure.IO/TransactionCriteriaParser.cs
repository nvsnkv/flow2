using System.ComponentModel;
using System.Globalization;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Flow.Domain.Patterns;
using Flow.Domain.Transactions;
using Flow.Infrastructure.IO.Contract;

namespace Flow.Infrastructure.IO;

/*
example: find ts[2021-02-02:2021-09-01] a<-1000 !c=RUB ocom%noway cat(,rose,blue) 
              timestamp between a:b
              amount <-1000
              currency != RUB
              comment LIKE noway
              category IN ('',rose, blue)

   ts    =val
!  a     %val
   c     <val
   cat   >val
   t     >=val
   acc   <=val
   bnk   [from:till]
   ocom  (from:till]
   ocat  [from:till)
   ot    (from:till)
   k     (val,val,val)


^(!?)(ts|a|c|cat|ttl|acc|bnk|ocom|ocat|ottl)(\=|\%|\<|\>|\<=|\>=|\[|\()(.*+)(\]|\))$
 */
internal class TransactionCriteriaParser : ITransactionCriteriaParser
{
    private readonly Regex criterionPattern = new(@"^(?<neg>!?)(?<prop>ts|a|c|cat|ttl|acc|bnk|ocom|ocat|ottl)(?<opStart>\=|\%|\<|\>|\<=|\>=|\[|\()(?<arg>.*)(?<opEnd>\]?|\)?)$");
    private readonly CultureInfo culture;

    public TransactionCriteriaParser(CultureInfo culture)
    {
        this.culture = culture;
    }

    public CriteriaParserResult<Transaction> ParseTransactionCriteria(IEnumerable<string> parts)
    {
        var builder = new PatternBuilder<Transaction>();
        var errors = new List<string>();

        foreach (var part in parts)
        {
            var (expr, err) = ParsePart<Transaction>(part);
            if (expr == null)
            {
                errors.Add(err!);
            }
            else
            {
                builder.With(expr);
            }
        }

        return new CriteriaParserResult<Transaction>(builder.Build(), errors);
    }

    public CriteriaParserResult<RecordedTransaction> ParseRecordedTransactionCriteria(IEnumerable<string> parts)
    {
        var builder = new PatternBuilder<RecordedTransaction>();
        var errors = new List<string>();

        foreach (var part in parts)
        {
            var (expr, err) = ParsePart<RecordedTransaction>(part);
            if (expr == null)
            {
                errors.Add(err!);
            }
            else
            {
                builder.With(expr);
            }
        }

        return new CriteriaParserResult<RecordedTransaction>(builder.Build(), errors);
    }

    private (Expression<Func<T,bool>>?, string?) ParsePart<T>(string part) where T : Transaction
    {
        var match = criterionPattern.Match(part);
        if (!match.Success)
        {
            return (null, $"Unable to parse criterion: {part}");
        }

        var prop = match.Groups["prop"];
        var opStart = match.Groups["opStart"];
        var arg = match.Groups["arg"];
        var opEnd = match.Groups["opEnd"];
        var neg = match.Groups["neg"];

        return prop.Value switch
        {
            "ts" => ParseTimestamp<T>(opStart, arg, opEnd, neg),
            "a" => ParseAmount<T>(opStart, arg, opEnd, neg),
            "c" => ParseCurrency<T>(opStart, arg, opEnd, neg),
            "cat" => ParseCategory<T>(opStart, arg, opEnd, neg),
            "t" => ParseTitle<T>(opStart, arg, opEnd, neg),
            "acc" => ParseAccount<T>(opStart, arg, opEnd, neg),
            "bnk" => ParseBank<T>(opStart, arg, opEnd, neg),
            "k" => ParseKey<T>(opStart, arg, opEnd, neg),
            "ocom" => ParseComment<T>(opStart, arg, opEnd, neg),
            "ocat" => ParseCategoryOverride<T>(opStart, arg, opEnd, neg),
            "ot" => ParseTitleOverride<T>(opStart, arg, opEnd, neg),
            _ => (null, $"Unknown property {prop} given!")
        };
    }

    private (Expression<Func<T, bool>>?, string?) ParseTitleOverride<T>(Group opStart, Group arg, Group opEnd, Group neg) where T : Transaction
    {
        if (!typeof(T).IsAssignableTo(typeof(RecordedTransaction)))
        {
            return (null, "unable to search by key in this context");
        }

        var (condition, err) = ParseScalar<string>(opStart, arg, opEnd, neg);
        if (condition == null) { return (null, err); }

        return (new PatternBuilder<T>().With(c => GetTitleOverride(c), condition).Build(), null);
    }

    private static string GetTitleOverride(Transaction transaction)
    {
        return (transaction as RecordedTransaction)?.Overrides?.Title ?? string.Empty;
    }

    private (Expression<Func<T, bool>>?, string?) ParseCategoryOverride<T>(Group opStart, Group arg, Group opEnd, Group neg) where T : Transaction
    {
        if (!typeof(T).IsAssignableTo(typeof(RecordedTransaction)))
        {
            return (null, "unable to search by key in this context");
        }

        var (condition, err) = ParseScalar<string>(opStart, arg, opEnd, neg);
        if (condition == null) { return (null, err); }

        return (new PatternBuilder<T>().With(c => GetCategoryOverride(c), condition).Build(), null);
    }

    private static string GetCategoryOverride(Transaction transaction)
    {
        return (transaction as RecordedTransaction)?.Overrides?.Category ?? string.Empty;
    }

    private (Expression<Func<T, bool>>?, string?) ParseComment<T>(Group opStart, Group arg, Group opEnd, Group neg) where T : Transaction
    {
        if (!typeof(T).IsAssignableTo(typeof(RecordedTransaction)))
        {
            return (null, "unable to search by key in this context");
        }

        var (condition, err) = ParseScalar<string>(opStart, arg, opEnd, neg);
        if (condition == null) { return (null, err); }
        
        return (new PatternBuilder<T>().With(c => GetComment(c), condition).Build(), null);
    }

    private static string GetComment(Transaction transaction)
    {
        return (transaction as RecordedTransaction)?.Overrides?.Comment ?? string.Empty;
    }

    private (Expression<Func<T, bool>>?, string?) ParseKey<T>(Group opStart, Group arg, Group opEnd, Group neg) where T : Transaction
    {
        if (!typeof(T).IsAssignableTo(typeof(RecordedTransaction)))
        {
            return (null, "unable to search by key in this context");
        }

        var (condition, err) = ParseScalar<long>(opStart, arg, opEnd, neg);
        if (condition == null) { return (null, err); }

        return (new PatternBuilder<T>().With(c => (c as RecordedTransaction)!.Key, condition).Build(), null);
    }

    private (Expression<Func<T, bool>>?, string?) ParseBank<T>(Group opStart, Group arg, Group opEnd, Group neg) where T : Transaction
    {
        var (condition, err) = ParseScalar<string>(opStart, arg, opEnd, neg);
        if (condition == null) { return (null, err); }

        return (new PatternBuilder<T>().With(t => t.Account.Bank, condition).Build(), null);
    }

    private (Expression<Func<T, bool>>?, string?) ParseAccount<T>(Group opStart, Group arg, Group opEnd, Group neg) where T : Transaction
    {
        var (condition, err) = ParseScalar<string>(opStart, arg, opEnd, neg);
        if (condition == null) { return (null, err); }

        return (new PatternBuilder<T>().With(t => t.Account.Name, condition).Build(), null);
    }

    private (Expression<Func<T, bool>>?, string?) ParseTitle<T>(Group opStart, Group arg, Group opEnd, Group neg) where T : Transaction
    {
        var (condition, err) = ParseScalar<string>(opStart, arg, opEnd, neg);
        if (condition == null) { return (null, err); }

        return (new PatternBuilder<T>().With(t => t.Title, condition).Build(), null);
    }

    private (Expression<Func<T, bool>>?, string?) ParseCategory<T>(Group opStart, Group arg, Group opEnd, Group neg) where T : Transaction
    {
        var (condition, err) = ParseScalar<string>(opStart, arg, opEnd, neg);
        if (condition == null) { return (null, err); }

        return (new PatternBuilder<T>().With(t => t.Category, condition).Build(), null);
    }

    private (Expression<Func<T, bool>>?, string?) ParseCurrency<T>(Group opStart, Group arg, Group opEnd, Group neg) where T : Transaction
    {
        var (condition, err) = ParseScalar<string>(opStart, arg, opEnd, neg);
        if (condition == null) { return (null, err); }

        return (new PatternBuilder<T>().With(t => t.Currency, condition).Build(), null);
    }

    private (Expression<Func<T, bool>>?, string?) ParseAmount<T>(Group opStart, Group arg, Group opEnd, Group neg) where T : Transaction
    {
        var (condition, err) = ParseScalar<decimal>(opStart, arg, opEnd, neg);
        if (condition == null) { return (null, err); }

        return (new PatternBuilder<T>().With(t => t.Amount, condition).Build(), null);
    }

    private (Expression<Func<T, bool>>?, string?) ParseTimestamp<T>(Group opStart, Group arg, Group opEnd, Group neg) where T : Transaction
    {
        var (condition, err) = ParseScalar<DateTime>(opStart, arg, opEnd, neg);
        if (condition == null) { return (null, err); }

        return (new PatternBuilder<T>().With(t => t.Timestamp, condition).Build(), null);
    }

    private (Expression<Func<T,bool>>?, string?) ParseScalar<T>(Group opStart, Group arg, Group opEnd, Group neg)
    {
        var value = arg.Value;
        var param = Expression.Parameter(typeof(T));

        (Expression<Func<T, bool>>?, string?) pair = opStart.Value switch
        {
            "=" => TryParseVal<T>(value, out var t) 
                ? (Expression.Lambda<Func<T, bool>>(Expression.Equal(param, Expression.Constant(t)), param), null) 
                : (null, $"Unable to parse value {arg} for type {typeof(T).Name}"),

            "%" => typeof(T) == typeof(string) ? 
                (BuildContainsExpression<T>(param, value), null) 
                : (null, $"Unable to parse value {arg} for type {typeof(T).Name} and operation {opStart.Value}"),

            "<" => TryParseVal<T>(value, out var t)
                ? (Expression.Lambda<Func<T, bool>>(Expression.LessThan(param, Expression.Constant(t)), param), null)
                : (null, $"Unable to parse value {arg} for type {typeof(T).Name}"),

            ">" => TryParseVal<T>(value, out var t)
                ? (Expression.Lambda<Func<T, bool>>(Expression.GreaterThan(param, Expression.Constant(t)), param), null)
                : (null, $"Unable to parse value {arg} for type {typeof(T).Name}"),

            "<=" => TryParseVal<T>(value, out var t)
                ? (Expression.Lambda<Func<T, bool>>(Expression.LessThanOrEqual(param, Expression.Constant(t)), param), null)
                : (null, $"Unable to parse value {arg} for type {typeof(T).Name}"),

            ">=" => TryParseVal<T>(value, out var t)
                ? (Expression.Lambda<Func<T, bool>>(Expression.GreaterThanOrEqual(param, Expression.Constant(t)), param), null)
                : (null, $"Unable to parse value {arg} for type {typeof(T).Name}"),

            "[" =>  TryParseMinMax<T>(value, out var min, out var max) 
                    ? opEnd.Value switch
                    {
                        "]" => (Expression.Lambda<Func<T, bool>>(Expression.Add(
                            Expression.GreaterThanOrEqual(Expression.Constant(min), param),
                            Expression.GreaterThanOrEqual(Expression.Constant(max), param)
                            ), param), null),
                        ")" => (Expression.Lambda<Func<T, bool>>(Expression.Add(
                            Expression.GreaterThanOrEqual(Expression.Constant(min), param),
                            Expression.GreaterThan(Expression.Constant(max), param)
                            ), param), null),
                        _ => (null, $"Unable to parse right boundary for {opStart.Value}{value}{opEnd.Value}")
                    }
                    : (null, $"Unable to parse boundaries for {opStart.Value}{value}{opEnd.Value}"),


            "(" => arg.Value.Contains(',')
                ? opEnd.Value == ")" && TryParseCollection<T>(value, out var collection)
                    ? (BuildContainsExpression(param, collection), null)
                    : (null, $"Unable to parse collection from {opStart.Value}{value}{opEnd.Value}")
                : TryParseMinMax<T>(value, out var min, out var max)
                ? opEnd.Value switch
                {
                    "]" => (Expression.Lambda<Func<T, bool>>(Expression.Add(
                        Expression.GreaterThanOrEqual(Expression.Constant(min), param),
                        Expression.GreaterThanOrEqual(Expression.Constant(max), param)
                    ), param), null),
                    ")" => (Expression.Lambda<Func<T, bool>>(Expression.Add(
                        Expression.GreaterThanOrEqual(Expression.Constant(min), param),
                        Expression.GreaterThan(Expression.Constant(max), param)
                    ), param), null),
                    _ => (null, $"Unable to parse right boundary for {opStart.Value}{value}{opEnd.Value}")
                }
                : (null, $"Unable to parse boundaries for {opStart.Value}{value}{opEnd.Value}"),

            _ => (null, $"Unknown operation {opStart}..{opEnd} given!")
        };

        var (expr, err) = pair;
        if (neg.Value == "!" && expr != null)
        {
            return (Expression.Lambda<Func<T, bool>>(Expression.Not(Expression.Invoke(expr, param)), param), err);
        }

        return pair;
    }

    private Expression<Func<T, bool>> BuildContainsExpression<T>(ParameterExpression param, string part)
    {
        Expression<Func<string, bool>> contains = s => s.Contains(part);
        return Expression.Lambda<Func<T, bool>>(contains.Body, param);
    }

    private Expression<Func<T, bool>> BuildContainsExpression<T>(ParameterExpression param, List<T> collection)
    {
        Expression<Func<T, bool>> contains = c => collection.Contains(c);
        return Expression.Lambda<Func<T, bool>>(contains.Body, param);
    }

    private bool TryParseCollection<T>(string value, out List<T> list)
    {
        var result = new List<T>();
        var converted = value.Split(',').Select(p =>
        {
            if (TryParseVal<T>(p, out var v))
            {
                result.Add(v!);
                return true;
            }

            return false;
        });

        list = result;
        return converted.All(c => c);
    }

    private bool TryParseMinMax<T>(string value, out T min, out T max)
    {
        min = max = default!;
        var split = value.Split(":");
        if (split.Length != 2) { return false; }

        return TryParseVal(split[0], out min!) && TryParseVal(split[1], out max!);
    }

    private bool TryParseVal<T>(string arg, out T? o)
    {
        o = default;

        try
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter.CanConvertTo(typeof(T)))
            {
                var res = (T?)converter.ConvertFromString(null, culture, arg);
                if (res != null)
                {
                    o = res;
                }

                return res != null;
            }

            return false;
        }
        catch (NotSupportedException)
        {
            return false;
        }
    }
}