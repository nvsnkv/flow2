using System.Text.RegularExpressions;
using Flow.Domain.Analysis;
using Flow.Domain.Transactions;

namespace Flow.Application.Analysis;

internal class Substitutor
{
    private readonly List<(Regex, Func<RecordedTransaction, string>)> substitutors;
    private readonly IComparer<Vector> vectorComparer;

    public Substitutor(IFormatProvider formatProvider, IComparer<Vector> vectorComparer)
    {
        this.vectorComparer = vectorComparer;
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

    public Dictionary<Vector, List<Vector>> SubstitutionsMade { get; } = new();
    public bool SubstitutionsSorted { get; private set; }

    public bool IsSubstitutionNeeded(Vector dimension)
    {
        return dimension.Any(d => substitutors.Any(s => s.Item1.IsMatch(d)));
    }
    
    public Vector Substitute(Vector dimension, RecordedTransaction transaction)
    {
        var result = new Vector(
            dimension.Select(
                d =>
                    substitutors.Aggregate(
                        d,
                        (r, p) => p.Item1.Replace(r, p.Item2(transaction))
                    )
            )
        );

        if (!SubstitutionsMade.ContainsKey(dimension))
        {
            SubstitutionsMade.Add(dimension, new List<Vector>());
        }

        if (!SubstitutionsMade[dimension].Contains(result))
        {
            SubstitutionsMade[dimension].Add(result);
        }

        return result;
    }

    public void SortSubstitutions()
    {
        foreach (var key in SubstitutionsMade.Keys)
        {
            SubstitutionsMade[key].Sort(vectorComparer);
        }

        SubstitutionsSorted = true;
    }
}