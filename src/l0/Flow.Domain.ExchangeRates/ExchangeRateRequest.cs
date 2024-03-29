﻿namespace Flow.Domain.ExchangeRates;

public class ExchangeRateRequest
{
    public ExchangeRateRequest(string from, string to, DateTime date)
    {
        Date = date;
        To = to;
        From = from;
    }

    public string From { get; }
    public string To { get; }
    public DateTime Date { get; }

    protected bool Equals(ExchangeRateRequest other)
    {
        return From == other.From && To == other.To && Date.Equals(other.Date);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((ExchangeRateRequest)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(From, To, Date);
    }

    public static implicit operator ExchangeRateRequest((string, string, DateTime) req)
    {
        var (from, to, date) = req;
        return new(from, to, date);
    }
}