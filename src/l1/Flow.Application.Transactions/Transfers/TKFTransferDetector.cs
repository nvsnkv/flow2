using Flow.Application.ExchangeRates.Contract;
using Flow.Domain.Transactions;
using Flow.Domain.Transactions.Transfers;

namespace Flow.Application.Transactions.Transfers;

internal sealed class TKFTransferDetector : TransferDetectorBase
{
    public TKFTransferDetector(IExchangeRatesProvider ratesProvider) : base("TKF internal transfer", ratesProvider, DetectionAccuracy.Exact)
    {
    }

    public override bool CheckIsTransfer(RecordedTransaction left, RecordedTransaction right)
    {
        return left.Timestamp == right.Timestamp
               && left.Amount < 0
               && right.Amount + left.Amount == 0
               && left.Category == "Переводы/иб"
               && left.Title == "Перевод между счетами"
               && right.Category == "Другое"
               && right.Title == "Перевод между счетами";
    }
}