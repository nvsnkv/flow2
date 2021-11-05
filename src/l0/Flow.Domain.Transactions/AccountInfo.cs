using System;

namespace Flow.Domain.Transactions
{
    public sealed class AccountInfo
    {
        public AccountInfo(string name, string bank)
        {
            Name = name;
            Bank = bank;
        }

        public string Name { get; }

        public string Bank { get; }

        private bool Equals(AccountInfo other)
        {
            return Name == other.Name && Bank == other.Bank;
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is AccountInfo other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Bank);
        }

        public static bool operator ==(AccountInfo? left, AccountInfo? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AccountInfo? left, AccountInfo? right)
        {
            return !Equals(left, right);
        }
    }
}