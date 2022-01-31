using Flow.Application.ExchangeRates.Contract;
using Flow.Domain.Transactions;
using Flow.Domain.Transactions.Transfers;
using FluentDateTime;

namespace Flow.Application.Transactions.Transfers;

class FuzzyTransferDetector : TransferDetectorBase
{
    private static readonly int TransferWindowDays = 3;
    public FuzzyTransferDetector(IExchangeRatesProvider ratesProvider) : base("May be a transfer", ratesProvider, DetectionAccuracy.Likely)
    {
    }

    public override bool CheckIsTransfer(RecordedTransaction left, RecordedTransaction right)
    {
        if (left.Amount > 0 || -1 * left.Amount != right.Amount)
        {
            return false;
        }

        if (left.Account == right.Account)
        {
            return false;
        }

        var leftTs = GetBusinessDate(left.Timestamp);
        var rightTs = GetBusinessDate(right.Timestamp);
        

        return leftTs <= rightTs && rightTs <= leftTs.AddBusinessDays(TransferWindowDays);
    }

    private static DateTime GetBusinessDate(DateTime timestamp)
    {
        return timestamp.DayOfWeek switch
        {
            DayOfWeek.Sunday => timestamp.Date.AddDays(1),
            DayOfWeek.Saturday => timestamp.Date.AddDays(2),
            _ => timestamp.Date
        };
    }
}