using Microsoft.Extensions.Options;
using OmniFi_API.DTOs.FreeCurrency;
using OmniFi_API.Options.Currencies;
using OmniFi_API.Services.Api;
using OmniFi_API.Models.Api;
using OmniFi_API.Services.Interfaces;
using System.Net;

namespace OmniFi_API.Services.Api.Currencies
{
    public class FiatCurrencyService : BaseService, IFiatCurrencyService
    {
        private readonly FiatCurrencyServiceOptions _options;

        private const string VersionTag = "{version}";
        private const string MethodTag = "{method}";
        private const string Endpoint = $"/{VersionTag}/{MethodTag}";

        private const string LastestExchangeRatesMethod = "latest";
        private const string CurrencyInfoMethod = "currencies";

        private const string ApiVersion = "v1";

        private const string ApiKey = "apikey";
        private const string BaseCurrencyKey = "base_currency";
        private const string CurrenciesKey = "currencies";

        public FiatCurrencyService(
            IHttpClientFactory httpClientFactory,
            IOptions<FiatCurrencyServiceOptions> options) : base(httpClientFactory)
        {
            _options = options.Value;
        }

        public async Task<decimal?> GetConversionRate(string inputCurrency, string targetCurrency)
        {
            var UrlWithQueryParameters = GetConversionUrlWithQueryParameters(inputCurrency, targetCurrency);

            var response = await GetLatestExchangeRates(UrlWithQueryParameters);

            return response is not null ?
                response.data[targetCurrency] :
                null;
        }

        public async Task<IEnumerable<CurrencyInfo>?> GetCurrenciesInfo(IEnumerable<string> currencyList)
        {
            string UrlWithQueryParameters = GetCurrenciesUrlWithQueryParameters(currencyList);

            var response = await GetCurrencyInfos(UrlWithQueryParameters);

            return response is not null ?
                response.data.Values :
                null;
        }

        private string GetCurrenciesUrlWithQueryParameters(IEnumerable<string> currencyList)
        {
            var queryParameters = string.Join(_options.Separator, currencyList);

            return _options.ApiBaseUrl
                + Endpoint.Replace(VersionTag, ApiVersion).Replace(MethodTag, CurrencyInfoMethod)
                + "?"
                + ApiKey + "=" + _options.ApiKey
                + "&"
                + CurrenciesKey + "=" + Uri.EscapeDataString(queryParameters);
        }

        private async Task<LatestExchangeRatesResponse?> GetLatestExchangeRates(string UrlQueryParameters)
        {

            return await SendAsync<LatestExchangeRatesResponse>(new ApiRequest()
            {
                ApiType = Utilities.ApiTypes.ApiType.GET,
                Url = UrlQueryParameters,
            });
        }

        private async Task<CurrenciesResponse?> GetCurrencyInfos(string UrlQueryParameters)
        {

            return await SendAsync<CurrenciesResponse>(new ApiRequest()
            {
                ApiType = Utilities.ApiTypes.ApiType.GET,
                Url = UrlQueryParameters,
            });
        }

        private string GetConversionUrlWithQueryParameters(string inputCurrency, string targetCurrency)
            => $"{_options.ApiBaseUrl + Endpoint.Replace(VersionTag, ApiVersion).Replace(MethodTag, LastestExchangeRatesMethod)}" +
               $"?{ApiKey}={Uri.EscapeDataString(_options.ApiKey)}" +
               $"&{CurrenciesKey}={Uri.EscapeDataString(targetCurrency)}" +
               $"&{BaseCurrencyKey}={Uri.EscapeDataString(inputCurrency)}";

    }
}
