﻿using System;

namespace Flow.Domain.Transactions
{
    public class Transaction
    {
        public Transaction(decimal amount, string currency, string? category, string title, AccountInfo account)
        {
            Amount = amount;
            Currency = currency;
            Category = category ?? string.Empty;
            Title = title;
            Account = account;
        }

        public Transaction(Transaction other)
        {
            Amount = other.Amount;
            Currency = other.Currency;
            Category = other.Category;
            Title = other.Title;
            Account = other.Account;
        }

        public decimal Amount { get; }

        public string Currency { get; }

        public string Category { get; }

        public string Title { get; }

        public AccountInfo Account { get; }

        protected bool Equals(Transaction other)
        {
            return Amount == other.Amount && Currency == other.Currency && Category == other.Category && Title == other.Title && Account.Equals(other.Account);
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Transaction)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Amount, Currency, Category, Title, Account);
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
}
