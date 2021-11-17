using System.Net.Http.Headers;
using Flow.Application.ExchangeRates.Contract;
using Flow.Application.ExchangeRates.Infrastructure;
using Flow.Domain.ExchangeRates;
using FluentValidation;

namespace Flow.Application.ExchangeRates;

internal class ExchangeRatesManager : IExchangeRatesManager
{
    private readonly IRemoteExchangeRatesProvider remote;
    private readonly IExchangeRatesStorage storage;
    private readonly IValidator<ExchangeRate> validator;
    private readonly IValidator<ExchangeRateRequest> requestValidator;

    public ExchangeRatesManager(IRemoteExchangeRatesProvider remote, IExchangeRatesStorage storage, IValidator<ExchangeRate> validator, IValidator<ExchangeRateRequest> requestValidator)
    {
        this.remote = remote;
        this.storage = storage;
        this.validator = validator;
        this.requestValidator = requestValidator;
    }

    public async Task<ExchangeRate?> Request(ExchangeRateRequest request, CancellationToken ct)
    {
        var valResult = await requestValidator.ValidateAsync(request, ct);
        if (!valResult.IsValid)
        {
            throw new ArgumentException("Given request is invalid!", nameof(request))
            {
                Data = { { "Errors", valResult.Errors.Select(e => e.ToString()) } }
            };
        }

        return await remote.GetRate(request, ct);
    }

    public async Task<IEnumerable<ExchangeRate>> List(CancellationToken ct)
    {
        return await storage.Read(ct);
    }

    public async Task<IEnumerable<RejectedRate>> Update(IEnumerable<ExchangeRate> rates, CancellationToken ct)
    {
        var rejections = new List<RejectedRate>();

        await storage.Update(rates.Where(r => Validate(r, rejections)), ct);

        return rejections;
    }

    public async Task<IEnumerable<RejectedRate>> Delete(IEnumerable<ExchangeRate> rates, CancellationToken ct)
    {
        var existing = (await storage.Read(ct)).ToList();
        var rejections = new List<RejectedRate>();

        await storage.Update(rates.Where(r => Validate(r, rejections)).Where(r => Exists(r, existing, rejections)), ct);

        return rejections;
    }

    private bool Validate(ExchangeRate exchangeRate, List<RejectedRate> rejections)
    {
        var valResult = validator.Validate(exchangeRate);
        if (!valResult.IsValid)
        {
            rejections.Add(new RejectedRate(exchangeRate, valResult.Errors.Select(e => e.ToString()).ToList().AsReadOnly()));
        }

        return valResult.IsValid;
    }

    private bool Exists(ExchangeRate rate, List<ExchangeRate> existing, List<RejectedRate> rejections)
    {
        var result = existing.Contains(rate);
        if (!result)
        {
            rejections.Add(new RejectedRate(rate, "Given rate does not exist in collection!"));
        }

        return result;
    }
}