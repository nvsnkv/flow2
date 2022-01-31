using Flow.Domain.Common;
using Flow.Domain.Transactions;
using FluentDateTime;

namespace Flow.Application.Transactions;

internal class DuplicateTransactionsComparer : IEqualityComparer<RecordedTransaction>
{
    private readonly int distance;

    public DuplicateTransactionsComparer(int distance = 3)
    {
        this.distance = distance;
    }

    public bool Equals(RecordedTransaction? x, RecordedTransaction? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;

        var xTs = x.Timestamp.BusinessDate();
        var yTs = y.Timestamp.BusinessDate();

        return xTs <= yTs && yTs <= xTs.AddBusinessDays(distance)
          && x.Amount == y.Amount
          && x.Currency == y.Currency
          && x.Category == y.Category
          && x.Title == y.Title
          && x.Account == y.Account;
    }

    public int GetHashCode(RecordedTransaction obj)
    {
        return obj.GetHashCode();
    }
}