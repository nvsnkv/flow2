using System.Globalization;
using System.Xml;
using Flow.Application.ExchangeRates.Infrastructure;
using Flow.Domain.ExchangeRates;

namespace Flow.Infrastructure.Rates.CBRF;
internal class CBRFRemoteRatesProvider : IRemoteExchangeRatesProvider
{
    private static readonly string HomelandCurrency = "RUB";
    private static readonly string RequestUrl = "http://www.cbr.ru/scripts/XML_daily.asp?date_req=";

    private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("ru-RU");

    public async Task<ExchangeRate?> GetRate(ExchangeRateRequest request, CancellationToken ct)
    {
        if (request.From != HomelandCurrency && request.To != HomelandCurrency) throw new ArgumentException("Unable to get exchange rate for conversions that does not involve RUB!", nameof(request));

        var foreignCurrency = request.From == HomelandCurrency ? request.To : request.From;

        var rate = await GetRate(foreignCurrency, request.Date, ct);

        return rate.HasValue ? new ExchangeRate(request.From, request.To, request.Date, rate.Value) : null;
    }

    private static async Task<decimal?> GetRate(string foreignCurrency, DateTime requestDate, CancellationToken ct)
    {
        var request = new Uri(RequestUrl + requestDate.ToString("dd/MM/yyyy"));
        using var client = new HttpClient();
        var stream = await client.GetStreamAsync(request, ct);
        using var reader = XmlReader.Create(stream);
        while (await reader.ReadAsync())
        {
            if (reader.Name == "Valute")
            {
                var matched = false;
                decimal? rate = null;
                while (await reader.ReadAsync())
                {
                    switch (reader.Name)
                    {
                        case "CharCode":
                            var currency = await reader.ReadElementContentAsStringAsync();
                            matched = currency == foreignCurrency;
                            break;
                        
                    
                        case "Value":
                            var value = await reader.ReadElementContentAsStringAsync();
                            rate = decimal.TryParse(value, NumberStyles.Any, Culture, out var d) ? d : null;
                            break;
                        
                    }

                    if (matched && rate.HasValue)
                    {
                        return rate;
                    }
                }
            }
        }

        return null;
    }
}