using System.Globalization;
using System.Xml;
using Flow.Application.ExchangeRates.Infrastructure;
using Flow.Domain.ExchangeRates;

namespace Flow.Infrastructure.Rates.CBRF;

internal class CBRFRemoteRatesProvider : IRemoteExchangeRatesProvider
{
    private static readonly Dictionary<string, string> CurrenciesMapping = new()
    {
        { "₽", "RUB" },
        { "$", "USD" }
    };

    private static readonly string HomelandCurrency = "RUB";
    private static readonly string RequestUrl = "http://www.cbr.ru/scripts/XML_daily.asp?date_req=";

    private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("ru-RU");

    public async Task<ExchangeRate?> GetRate(ExchangeRateRequest request, CancellationToken ct)
    {
        if (IsHomelandCurrency(request.From) && IsHomelandCurrency(request.To)) throw new ArgumentException("Unable to get exchange rate for conversions that does not involve RUB!", nameof(request));

        var foreignCurrency = IsHomelandCurrency(request.From) ? request.To : request.From;

        var rate = await GetRate(foreignCurrency, request.Date, ct);
        if (!rate.HasValue) return null;

        if (!IsHomelandCurrency(request.To))
        {
            rate = 1 / rate;
        }

        return new ExchangeRate(request.From, request.To, request.Date, rate.Value);
    }

    private static async Task<decimal?> GetRate(string foreignCurrency, DateTime requestDate, CancellationToken ct)
    {
        foreignCurrency = CurrenciesMapping.ContainsKey(foreignCurrency)
            ? CurrenciesMapping[foreignCurrency]
            : foreignCurrency;

        var request = new Uri(RequestUrl + requestDate.ToString("dd/MM/yyyy"));
        using var client = new HttpClient();
        var stream = await client.GetStreamAsync(request, ct);
        using var reader = XmlReader.Create(stream, new XmlReaderSettings { Async = true });
        while (await reader.ReadAsync())
        {
            if (reader.Name == "Valute")
            {
                var currency = string.Empty;
                decimal? rate = null;
                while (await reader.ReadAsync())
                {
                    switch (reader.Name)
                    {
                        case "CharCode":
                            currency = await reader.ReadElementContentAsStringAsync();
                            break;


                        case "Value":
                            var value = await reader.ReadElementContentAsStringAsync();
                            rate = decimal.TryParse(value, NumberStyles.Any, Culture, out var d) ? d : null;
                            break;

                    }

                    if (currency == foreignCurrency && rate.HasValue)
                    {
                        return rate;
                    }

                    if (!string.IsNullOrEmpty(currency) && rate.HasValue)
                    {
                        break;
                    }

                }
            }
        }

        return null;
    }

    private static bool IsHomelandCurrency(string currency) => currency == HomelandCurrency || CurrenciesMapping.ContainsKey(currency) && CurrenciesMapping[currency] == HomelandCurrency;
}