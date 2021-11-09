using System;

namespace Flow.Domain.Transactions;

public class Transaction
{
    public Transaction(DateTime timestamp, decimal amount, string currency, string? category, string title, AccountInfo account)
    {
        Amount = amount;
        Currency = currency;
        Category = category ?? string.Empty;
        Title = title;
        Account = account;
        Timestamp = timestamp;
    }

    public Transaction(Transaction other)
    {
        Timestamp = other.Timestamp;
        Amount = other.Amount;
        Currency = other.Currency;
        Category = other.Category;
        Title = other.Title;
        Account = other.Account;
    }
        
    public DateTime Timestamp { get; }

    public decimal Amount { get; }

    public string Currency { get; }

    public string Category { get; }

    public string Title { get; }

    public virtual AccountInfo Account { get; }

    protected bool Equals(Transaction other)
    {
        return Timestamp == other.Timestamp && Amount == other.Amount && Currency == other.Currency && Category == other.Category && Title == other.Title && Account.Equals(other.Account);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((Transaction)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Timestamp, Amount, Currency, Category, Title, Account);
    }

    public static bool operator ==(Transaction? left, Transaction? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Transaction? left, Transaction? right)
    {
        return !Equals(left, right);
    }
}