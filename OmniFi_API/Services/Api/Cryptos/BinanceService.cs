using Elfie.Serialization;
using Microsoft.Extensions.Configuration;
using OmniFi_API.DTOs.Binance;
using OmniFi_API.Models.Api;
using OmniFi_API.Models.Portfolio;
using OmniFi_API.Services.Interfaces;
using OmniFi_API.Utilities;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using static OmniFi_API.Utilities.ApiTypes;

namespace OmniFi_API.Services.Api.Cryptos
{
    public class BinanceService : BaseService, IFinancialAssetService
    {
        private const string DefaultSpotBaseUrl = "https://api.binance.com";
        private const string DefaultFuturesBaseUrl = "https://testnet.binancefuture.com";

        private const string ConfigSpotBaseUrlIndex = "ApiBaseUrls:BinanceSpot";
        private const string ConfigFuturesBaseUrlIndex = "ApiBaseUrls:BinanceFutures";

        private readonly string _binanceSpotBaseUrl;
        private readonly string _binanceFuturesBaseUrl;

        private const string MethodTag = "{method}";
        private const string Endpoint = $"{MethodTag}";

        private const string SpotUserBalanceMethod = "/api/v3/account";
        private const string SymbolPriceTickerMethod = "/api/v3/ticker/price";

        private const string ApiKeyHeaderKey = "X-MBX-APIKEY";

        private const string TimestampParameter = "timestamp";
        private const string OmitZeroBalancesParameter = "omitZeroBalances";
        private const string SignatureParameter = "signature";

        private const string BinanceDefaultCurrency = FiatCurrencyCodes.USD;
        private const string BinanceDefaultStable = "USDT";

        private readonly List<string> _symbolsNotHandled = new()
        {
            "NFT",
            "ETHW"
        };

        public BinanceService(IHttpClientFactory httpClientFactory, IConfiguration configuration) : base(httpClientFactory)
        {
            _binanceSpotBaseUrl = configuration
                .GetValue<string>(ConfigSpotBaseUrlIndex) ?? DefaultSpotBaseUrl;

            _binanceFuturesBaseUrl = configuration
                .GetValue<string>(ConfigFuturesBaseUrlIndex) ?? DefaultFuturesBaseUrl;
        }

        public async Task<IEnumerable<PortfolioData>?> GetUserBalanceAsync(string ApiKey, string ApiSecret)
        {
            var userBalanceResponse = await GetUserSpotBalance(ApiKey, ApiSecret);

            return userBalanceResponse is not null ?
                ParseUserSpotBalance(userBalanceResponse, await GetPairsTicker()) :
                null;
            
        }

        private IEnumerable<PortfolioData>? ParseUserSpotBalance(UserBalanceResponse userBalanceResponse, Dictionary<string, decimal> tickerPrices)
        {
            List<PortfolioData>? result = new();

          

            foreach (var balance in userBalanceResponse.balances)
            {
                if (_symbolsNotHandled.Contains(balance.asset))
                    continue;

                decimal value = 0;
                var pairFiat = balance.asset + BinanceDefaultCurrency;
                var pairStable = balance.asset + BinanceDefaultStable;
                var sumedConvertedBalance = decimal.Parse(balance.free) + decimal.Parse(balance.locked);

                if (balance.asset.Contains("USD"))
                {
                    value = sumedConvertedBalance;
                }
                else if (tickerPrices.ContainsKey(pairStable))
                {
                    value = sumedConvertedBalance * tickerPrices[pairStable];
                }
                else if (tickerPrices.ContainsKey(pairFiat))
                {
                    value = sumedConvertedBalance * tickerPrices[pairFiat];
                }
                else
                {
                    throw new Exception($"The pairs {pairStable} and {pairFiat} does not exists in Binance tickers list");
                }

                result.Add(new PortfolioData()
                {
                    AssetPlatformName = CryptoExchangeNames.Binance,
                    AssetSourceName = AssetSourceNames.CryptoHolding,
                    CryptoCurrencySymbol = balance.asset,
                    Quantity = sumedConvertedBalance,
                    Balance = value,
                    FiatCurrencyCode = BinanceDefaultCurrency
                });

            }

            return result;
        }

        private async Task<UserBalanceResponse?> GetUserSpotBalance(string apiKey, string apiSecret)
        {
            Dictionary<string, string> headersParameters = new()
            {
                [ApiKeyHeaderKey] = apiKey
            };

            var nonce = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

            var requestData = new Dictionary<string, string>()
            {
                [TimestampParameter] = nonce,
                [OmitZeroBalancesParameter] = "true",
            };

            var urlPath = _binanceSpotBaseUrl +
                    SpotUserBalanceMethod
                    .Replace(MethodTag, SpotUserBalanceMethod);

            return await SendAsync<UserBalanceResponse>(new ApiRequest()
            {
                ApiType = ApiType.GET,
                Url = GetParameteriseddUrl(apiSecret, requestData, urlPath),
                HeaderDictionary = headersParameters
            });
        }

        private async Task<Dictionary<string, decimal>> GetPairsTicker()
        {
            var result = new Dictionary<string, decimal>();

            var endPoint = Endpoint.Replace(MethodTag, SymbolPriceTickerMethod);

            var tickerInfoResponse = await SendAsync<List<TickerInfoResponse>>(new ApiRequest()
            {
                ApiType = ApiType.GET,
                Url = _binanceSpotBaseUrl + endPoint,
            });

            if (tickerInfoResponse is not null)
            {
                foreach (var elem in tickerInfoResponse)
                {
                    result.Add(elem.symbol, decimal.Parse(elem.price));
                }
            }

            return result;
        }

        private string GetParameteriseddUrl(string apiSecret, Dictionary<string, string> requestData, string urlPath)
        {
            StringBuilder requestUriBuilder = new StringBuilder(urlPath);

            return requestUriBuilder
                .Append("?")
                .Append(GetSignature(apiSecret, requestData))
                .ToString();
        }

        private string GetSignature(string apiSecret, Dictionary<string, string> requestData)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var payload = string.Join("&",
                requestData.Select(x => x.Key + "=" + x.Value));

            stringBuilder.Append(payload);

            byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);
            byte[] keyBytes = Encoding.UTF8.GetBytes(apiSecret);

            using(var hmac256 = new HMACSHA256(keyBytes))
            {
                byte[] sourceBytes = Encoding.UTF8.GetBytes(payload);

                byte[] hash = hmac256.ComputeHash(sourceBytes);

                var signature = BitConverter
                    .ToString(hash)
                    .Replace("-","")
                    .ToLower();

                stringBuilder
                    .Append("&")
                    .Append(SignatureParameter + "=" + signature);
            }

    
            return stringBuilder.ToString();


        }
    }
}
