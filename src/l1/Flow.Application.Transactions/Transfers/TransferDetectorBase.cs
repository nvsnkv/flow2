using Flow.Application.ExchangeRates.Contract;
using Flow.Domain.Transactions;
using Flow.Domain.Transactions.Transfers;

namespace Flow.Application.Transactions.Transfers;

internal abstract class TransferDetectorBase : ITransferDetector
{
    private readonly string comment;
    private readonly IExchangeRatesProvider ratesProvider;

    protected TransferDetectorBase(string comment, IExchangeRatesProvider ratesProvider, DetectionAccuracy accuracy)
    {
        this.comment = comment;
        this.ratesProvider = ratesProvider;
        Accuracy = accuracy;
    }

    public abstract bool CheckIsTransfer(RecordedTransaction left, RecordedTransaction right);

    public async Task<Transfer> Create(RecordedTransaction left, RecordedTransaction right, CancellationToken ct)
    {
        if (!CheckIsTransfer(left, right))
        {
            throw new InvalidOperationException("Given transactions does not belong to a single transfer!");
        }

        if (left.Currency != right.Currency)
        {
            var rate = await ratesProvider.GetRate((right.Currency, left.Currency, left.Timestamp), ct);
            if (rate == null) { throw new ArgumentException("Failed to get exchange rate for transfer!"); }

            return new Transfer(left, right, Accuracy, left.Amount + right.Amount * rate.Rate, left.Currency)
            {
                Comment = comment + " (converted)"
            };
        }

        return new Transfer(left, right, DetectionAccuracy.Exact)
        {
            Comment = comment
        };
    }

    public DetectionAccuracy Accuracy { get; }
}