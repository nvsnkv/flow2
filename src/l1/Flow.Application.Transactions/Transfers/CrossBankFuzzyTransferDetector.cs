using Flow.Application.ExchangeRates.Contract;
using Flow.Domain.Transactions;
using Flow.Domain.Transactions.Transfers;
using FluentDateTime;

namespace Flow.Application.Transactions.Transfers;

class CrossBankFuzzyTransferDetector : TransferDetectorBase
{
    private static readonly int TransferWindowDays = 3;
    public CrossBankFuzzyTransferDetector(IExchangeRatesProvider ratesProvider) : base("May be a transfer", ratesProvider, DetectionAccuracy.Likely)
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
        

        if (left.Timestamp.Date.AddBusinessDays(TransferWindowDays) < right.Timestamp)
        {
            return false;
        }

        return true;
        
    }
}