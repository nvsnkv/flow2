using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using Flow.Domain.Patterns;
using Flow.Domain.Transactions;
using Flow.Infrastructure.IO.Contract;

namespace Flow.Infrastructure.IO.Criteria;

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
    private readonly GenericParser parser;
    private readonly Regex criterionPattern = new(@"^(?<neg>!?)(?<prop>ts?|a|cat|acc|c|bnk|ocom|ocat|ot|k)(?<opStart>\=|\%|\<=?|\>=?|\[|\()(?<arg>.*?)(?<opEnd>\]?|\)?)$");
    
    public TransactionCriteriaParser(GenericParser parser)
    {
        this.parser = parser;
    }

    public CriteriaParserResult<RecordedTransaction> ParseRecordedTransactionCriteria(string input)
    {
        return ParseRecordedTransactionCriteria(GetParts(input));
    }

    private static IEnumerable<string> GetParts(string part)
    {
        var builder = new StringBuilder();
        var i = 0;
        var quotes = false;

        while (i < part.Length)
        {
            if (part[i] == '"')
            {
                quotes = !quotes;
            }
            else if (part[i] == ' ' && !quotes)
            {
                var result = builder.ToString();
                builder.Clear();

                if (!string.IsNullOrEmpty(result))
                {
                    yield return result;
                }
            }
            else
            {
                builder.Append(part[i]);
            }

            i++;
        }

        yield return builder.ToString();
    }

    private CriteriaParserResult<RecordedTransaction> ParseRecordedTransactionCriteria(IEnumerable<string> parts)
    {
        var builder = new AndPatternBuilder<RecordedTransaction>();
        var errors = new List<string>();

        foreach (var part in parts)
        {
            var result = ParseCondition(part, builder);
            if (!result.Successful)
            {
                errors.Add(result.Error);
            }
        }

        return errors.Any() 
            ? new  CriteriaParserResult<RecordedTransaction>(errors)
            : new CriteriaParserResult<RecordedTransaction>(builder.Build());
    }

    private ParsingResult ParseCondition(string part, AndPatternBuilder<RecordedTransaction> builder)
    {
        var match = criterionPattern.Match(part);
        if (!match.Success)
        {
            return new ParsingResult($"Unable to parse criterion: {part}");
        }

        var prop = match.Groups["prop"];
        var opStart = match.Groups["opStart"];
        var arg = match.Groups["arg"];
        var opEnd = match.Groups["opEnd"];
        var neg = match.Groups["neg"];
        
        return prop.Value switch
        {
            "ts" => HandleDateTime(t => t.Timestamp, builder, opStart, arg, opEnd, neg),
            "a" => HandleDecimal(t => t.Amount, builder, opStart, arg, opEnd, neg),
            "c" => HandleString(t => t.Currency, builder, opStart, arg, opEnd, neg),
            "cat" => HandleString(t => t.Category, builder, opStart, arg, opEnd, neg),
            "t" => HandleString(t => t.Title, builder, opStart, arg, opEnd, neg),
            "acc" => HandleString(t => t.Account.Name, builder, opStart, arg, opEnd, neg),
            "bnk" => HandleString(t => t.Account.Bank, builder, opStart, arg, opEnd, neg),
            "k" => HandleLong(t => t.Key, builder, opStart, arg, opEnd, neg),
            "ocom" => HandleString(t => t.Overrides!.Comment, builder, opStart, arg, opEnd, neg),
            "ocat" => HandleString(t => t.Overrides!.Category, builder, opStart, arg, opEnd, neg),
            "ot" => HandleString(t => t.Overrides!.Title, builder, opStart, arg, opEnd, neg),
            _ => new ParsingResult($"Unknown property {prop} given!")
        };
    }

    private ParsingResult HandleDateTime<T>(Expression<Func<T, DateTime>> selector, AndPatternBuilder<T> builder, Group opStart, Group arg, Group opEnd, Group neg) where T : Transaction
    {
        var result = ParsePropertyCondition(selector, ParseAttributeConditions<DateTime>(opStart, arg, opEnd, neg));
        if (result.Successful)
        {
            builder.With(result.Selector!, result.Condition!);
        }

        return result;
    }

    private ParsingResult HandleDecimal<T>(Expression<Func<T, decimal>> selector, AndPatternBuilder<T> builder, Group opStart, Group arg, Group opEnd, Group neg) where T : Transaction
    {
        var result = ParsePropertyCondition(selector, ParseAttributeConditions<decimal>(opStart, arg, opEnd, neg));
        if (result.Successful)
        {
            builder.With(result.Selector!, result.Condition!);
        }

        return result;
    }
    
    private ParsingResult HandleLong<T>(Expression<Func<T, long>> selector, AndPatternBuilder<T> builder, Group opStart, Group arg, Group opEnd, Group neg) where T : Transaction
    {
        var result = ParsePropertyCondition(selector, ParseAttributeConditions<long>(opStart, arg, opEnd, neg));
        if (result.Successful)
        {
            builder.With(result.Selector!, result.Condition!);
        }

        return result;
    }

    private ParsingResult HandleString<T>(Expression<Func<T, string?>> selector, AndPatternBuilder<T> builder, Group opStart, Group arg, Group opEnd, Group neg) where T : Transaction
    {
        var result = ParsePropertyCondition(selector, ParseAttributeConditions<string?>(opStart, arg, opEnd, neg));
        if (result.Successful)
        {
            builder.With(result.Selector!, result.Condition!);
        }

        return result;
    }

    private static ConditionParseResult<T,TProp> ParsePropertyCondition<T, TProp>(Expression<Func<T, TProp>> selector, ParsingResult<TProp> parseResult) where T : Transaction
    {
        return parseResult.Successful 
            ? new ConditionParseResult<T, TProp>(selector, parseResult.Condition) 
            : new ConditionParseResult<T, TProp>(parseResult.Error);
    }

    private ParsingResult<T> ParseAttributeConditions<T>(Group opStart, Group arg, Group opEnd, Group neg)
    {
        var value = arg.Value;
        var param = Expression.Parameter(typeof(T));

        var result = opStart.Value switch
        {
            "=" => parser.TryParseSingleValue<T>(value, out var t) 
                ? new ParsingResult<T>(Expression.Lambda<Func<T, bool>>(Expression.Equal(param, Expression.Constant(t)), param))
                : new ParsingResult<T>($"Unable to parse value {arg} for type {typeof(T).Name}"),

            "%" => typeof(T) == typeof(string) ?
                new ParsingResult<T>(BuildContainsExpression<T>(param, value))
                : new ParsingResult<T>($"Unable to parse value {arg} for type {typeof(T).Name} and operation {opStart.Value}"),

            "<" => parser.TryParseSingleValue<T>(value, out var t)
                ? new ParsingResult<T>(Expression.Lambda<Func<T, bool>>(Expression.LessThan(param, Expression.Constant(t)), param))
                : new ParsingResult<T>($"Unable to parse value {arg} for type {typeof(T).Name}"),

            ">" => parser.TryParseSingleValue<T>(value, out var t)
                ? new ParsingResult<T>(Expression.Lambda<Func<T, bool>>(Expression.GreaterThan(param, Expression.Constant(t)), param))
                : new ParsingResult<T>($"Unable to parse value {arg} for type {typeof(T).Name}"),

            "<=" => parser.TryParseSingleValue<T>(value, out var t)
                ? new ParsingResult<T>(Expression.Lambda<Func<T, bool>>(Expression.LessThanOrEqual(param, Expression.Constant(t)), param))
                : new ParsingResult<T>($"Unable to parse value {arg} for type {typeof(T).Name}"),

            ">=" => parser.TryParseSingleValue<T>(value, out var t)
                ? new ParsingResult<T>(Expression.Lambda<Func<T, bool>>(Expression.GreaterThanOrEqual(param, Expression.Constant(t)), param))
                : new ParsingResult<T>($"Unable to parse value {arg} for type {typeof(T).Name}"),

            "[" => parser.TryParseBoundaries<T>(value, out var min, out var max) && (opEnd.Value == "]" || opEnd.Value == ")")
                ? new ParsingResult<T>(BuildMinMaxExpression(opStart.Value, opEnd.Value, min, max, param))
                : new ParsingResult<T>($"Unable to parse boundaries for {opStart.Value}{value}{opEnd.Value}"),


            "(" => arg.Value.Contains(',')
                ? opEnd.Value == ")" && parser.TryParseCollection<T>(value, out var collection)
                    ? new ParsingResult<T>(BuildContainsExpression(param, collection))
                    : new ParsingResult<T>($"Unable to parse collection from {opStart.Value}{value}{opEnd.Value}")
                : parser.TryParseBoundaries<T>(value, out var min, out var max) && (opEnd.Value == "]" || opEnd.Value == ")")
                    ? new ParsingResult<T>(BuildMinMaxExpression(opStart.Value, opEnd.Value, min, max, param))
                    : new ParsingResult<T>($"Unable to parse boundaries for {opStart.Value}{value}{opEnd.Value}"),

            _ => new ParsingResult<T>($"Unknown operation {opStart}..{opEnd} given!")
        };

        if (result.Successful && neg.Value == "!")
        {
            var expr = result.Condition!;
            return new ParsingResult<T>(Expression.Lambda<Func<T, bool>>(Expression.Not(Expression.Invoke(expr, param)), param));
        }

        return result;
    }

    private static Expression<Func<T, bool>> BuildMinMaxExpression<T>(string opStart, string opEnd, T min, T max, ParameterExpression param)
    {
        return Expression.Lambda<Func<T, bool>>(
            Expression.And(
                BuildMinExpression(opStart, min, param),
                BuildMaxExpression(opEnd, max, param)), 
            param);
    }

    private static Expression BuildMaxExpression<T>(string opEnd, T max, ParameterExpression param)
    {
        return opEnd switch
        {
            "]" => Expression.GreaterThanOrEqual(Expression.Constant(max), param),
            ")" => Expression.GreaterThan(Expression.Constant(max), param),
            _ => throw new NotSupportedException($"Not supported boundary {opEnd}!")
        };
    }

    private static Expression BuildMinExpression<T>(string opStart, T min, ParameterExpression param)
    {
        return opStart switch
        {
            "[" => Expression.LessThanOrEqual(Expression.Constant(min), param),
            "(" => Expression.LessThan(Expression.Constant(min), param),
            _ => throw new NotSupportedException($"Not supported boundary {opStart}!")
        };
    }

    private static Expression<Func<T, bool>> BuildContainsExpression<T>(ParameterExpression param, string? part)
    {
        var partAccess = Expression.Constant(part ?? string.Empty);
        return Expression.Lambda<Func<T, bool>>(Expression.Call(param, "Contains", null, partAccess), param);
    }

    private static Expression<Func<T, bool>> BuildContainsExpression<T>(ParameterExpression param, List<T> collection)
    {
        var collectionAccess = Expression.Constant(collection);
        return Expression.Lambda<Func<T, bool>>(Expression.Call(collectionAccess, "Contains", null, param), param);
    }

    
}