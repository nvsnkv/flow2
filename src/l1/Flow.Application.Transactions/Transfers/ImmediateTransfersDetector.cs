using Flow.Application.ExchangeRates.Contract;
using Flow.Domain.Transactions;
using Flow.Domain.Transactions.Transfers;

namespace Flow.Application.Transactions.Transfers;

internal class ImmediateTransfersDetector : TransferDetectorBase
{
    public override bool CheckIsTransfer(RecordedTransaction left, RecordedTransaction right)
    {
        var timeGap = right.Timestamp - left.Timestamp;
        return TimeSpan.Zero <= timeGap 
               && timeGap < TimeSpan.FromMinutes(2)
               && left.Amount == -1 * right.Amount
               && left.Amount < 0
               && left.Currency == right.Currency
               && left.Category == right.Category
               && left.Title == right.Title
               && left.Account != right.Account;
    }
    
    public ImmediateTransfersDetector(IExchangeRatesProvider ratesProvider) : base("Immediate transfer", ratesProvider, DetectionAccuracy.Exact)
    {
    }
}