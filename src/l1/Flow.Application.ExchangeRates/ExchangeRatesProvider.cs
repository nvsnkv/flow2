using Flow.Application.ExchangeRates.Contract;
using Flow.Application.ExchangeRates.Infrastructure;
using Flow.Domain.ExchangeRates;

namespace Flow.Application.ExchangeRates;

internal class ExchangeRatesProvider : IExchangeRatesProvider
{
    private readonly ManualResetEventSlim dataLoaded = new();
    private int loading;
    private Dictionary<string, Dictionary<string, Dictionary<DateTime, ExchangeRate>>>? exchangeRates;

    private readonly IExchangeRatesStorage storage;
    private readonly IRemoteExchangeRatesProvider remote;


    public ExchangeRatesProvider(IExchangeRatesStorage storage, IRemoteExchangeRatesProvider remote)
    {
        this.storage = storage;
        this.remote = remote;
    }

    public async Task<ExchangeRate?> GetRate(ExchangeRateRequest request, CancellationToken ct)
    {
        await EnsureRatesLoaded(ct);
        if (ct.IsCancellationRequested) { return null; }

        var result = GetFromLoaded(request, ct);
        if (result == null)
        {
            var remoteItem = await remote.GetRate(request, ct);
            if (ct.IsCancellationRequested) { return null; }

            if (remoteItem != null)
            {
                SetToLoaded(remoteItem);

                var _ = storage.Create(remoteItem, ct);
                if (ct.IsCancellationRequested) { return null; }

                result = remoteItem;
            }
        }

        return result;
    }

    private ExchangeRate? GetFromLoaded(ExchangeRateRequest req, CancellationToken ct)
    {
        dataLoaded.Wait(ct);
        if (ct.IsCancellationRequested) { return null; }

        if (!(exchangeRates?.ContainsKey(req.From) ?? false)) { return null; }
        var tosDict = exchangeRates[req.From];
        
        if (!tosDict.ContainsKey(req.To)) { return null; }
        var datesDict = tosDict[req.To];

        return !datesDict.ContainsKey(req.Date) ? null : datesDict[req.Date];
    }

    private void SetToLoaded(ExchangeRate rate)
    {
        dataLoaded.Reset();
        try
        {
            exchangeRates ??= new Dictionary<string, Dictionary<string, Dictionary<DateTime, ExchangeRate>>>();
            if (!exchangeRates.ContainsKey(rate.From))
            {
                exchangeRates.Add(rate.From, new Dictionary<string, Dictionary<DateTime, ExchangeRate>>());
            }

            var tosDictionary = exchangeRates[rate.From];
            if (!tosDictionary.ContainsKey(rate.To))
            {
                tosDictionary.Add(rate.To, new Dictionary<DateTime, ExchangeRate>());
            }

            tosDictionary[rate.To].Add(rate.Date, rate);
        }
        finally
        {
            dataLoaded.Set();
        }
    }

    private async Task EnsureRatesLoaded(CancellationToken ct)
    {
        if (exchangeRates != null) { return; }

        if (Interlocked.CompareExchange(ref loading, 1, 0) == 0)
        {
            try
            {
                var data = storage.Read(ct);

                exchangeRates = await data.GroupBy(r => r.From)
                    .ToDictionaryAwaitAsync(
                        gFrom => ValueTask.FromResult(gFrom.Key), 
                        gFrom => gFrom.GroupBy(r => r.To)
                            .ToDictionaryAwaitAsync(
                                gBy => ValueTask.FromResult(gBy.Key), 
                                gBy => gBy.ToDictionaryAsync(r => r.Date, ct),
                                ct),
                        ct);
            }
            finally
            {
                loading = 0;
                dataLoaded.Set();
            }
        }
        else
        {
            dataLoaded.Wait(ct);
        }
    }
}