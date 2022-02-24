using System.Text;
using System.Text.RegularExpressions;

namespace Flow.Infrastructure.IO.Criteria;

internal class CriteriaInputHandler
{
    private static readonly string Quotes = "'\"";
    private static readonly Regex QuotesPattern = new($"[{Quotes}]", RegexOptions.Compiled);
    private static readonly string Whitespaces = " ";
    
    public static IEnumerable<string> SplitAndUnquote(IEnumerable<string> input)
    {
        foreach (var item in input)
        {
            foreach (var interim in Split(item))
            {
                yield return QuotesPattern.Replace(interim, string.Empty);
            }
        }
    }

    private static IEnumerable<string> Split(string input)
    {
        var builder = new StringBuilder();
        var i = 0;
        char? quote = null;
        string part;

        while (i < input.Length)
        {
            if (!quote.HasValue && Quotes.Contains(input[i]))
            {
                quote = input[i];
            }
            else if (quote.HasValue && quote == input[i])
            {
                quote = null;
            }
            else if (!quote.HasValue && Whitespaces.Contains(input[i]))
            {
                part = builder.ToString();
                if (!string.IsNullOrEmpty(part))
                {
                    yield return part;
                }

                builder.Clear();
            }
            else
            {
                builder.Append(input[i]);
            }

            i++;
        }

        part = builder.ToString();
        if (!string.IsNullOrEmpty(part))
        {
            yield return part;
        }
    }
}