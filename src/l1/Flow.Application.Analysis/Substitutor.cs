using System.Text.RegularExpressions;
using Flow.Domain.Analysis;
using Flow.Domain.Transactions;

namespace Flow.Application.Analysis;

internal class Substitutor
{
    private readonly List<(Regex, Func<RecordedTransaction, string>)> substitutors;
    
    public Substitutor(IFormatProvider formatProvider)
    {
        substitutors = new List<(Regex, Func<RecordedTransaction, string>)>
        {
            (new Regex(@"\$ts", RegexOptions.Compiled), t => t.Timestamp.ToString(formatProvider)),
            (new Regex(@"\$t", RegexOptions.Compiled), t => t.Title),
            (new Regex(@"\$a", RegexOptions.Compiled), t => t.Amount.ToString(formatProvider)),
            (new Regex(@"\$cat", RegexOptions.Compiled), t => t.Category),
            (new Regex(@"\$acc", RegexOptions.Compiled), t => t.Account.Name),
            (new Regex(@"\$c", RegexOptions.Compiled), t => t.Currency),
            (new Regex(@"\$bnk", RegexOptions.Compiled), t => t.Account.Bank),
            (new Regex(@"\$ocat", RegexOptions.Compiled), t => t.Overrides?.Category ?? string.Empty),
            (new Regex(@"\$ocom", RegexOptions.Compiled), t => t.Overrides?.Comment ?? string.Empty),
            (new Regex(@"\$ot", RegexOptions.Compiled), t => t.Overrides?.Title ?? string.Empty),
            (new Regex(@"\$k", RegexOptions.Compiled), t => t.Key.ToString(formatProvider)),
        };
    }

    public bool IsSubstitutionNeeded(Vector source)
    {
        return source.Any(d => substitutors.Any(s => s.Item1.IsMatch(d)));
    }
    
    public Vector Substitute(Vector source, RecordedTransaction transaction)
    {
        var result = new Vector(
            source.Select(
                d =>
                    substitutors.Aggregate(
                        d,
                        (r, p) => p.Item1.Replace(r, p.Item2(transaction))
                    )
            )
        );

        return result;
    }
}