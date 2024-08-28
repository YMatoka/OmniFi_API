
using Microsoft.AspNetCore.DataProtection;
using OmniFi_API.DTOs.Kraken;
using OmniFi_API.Models.Api;
using OmniFi_API.Models.Portfolio;
using OmniFi_API.Services.Interfaces;
using static OmniFi_API.Utilities.ApiTypes;
using System.Security.Cryptography;
using System.Text;
using OmniFi_API.Utilities;
using System;
using NuGet.Common;
using System.Net.Sockets;
using Azure.Core;
using Newtonsoft.Json.Linq;

namespace OmniFi_API.Services.Api.Cryptos
{
    public class KrakenService : BaseService, IFinancialAssetService
    {
        private const string DefaultBaseUrl = "https://api.kraken.com";
        private const string ConfigBaseUrlIndex = "CryptoApiBaseUrls:Kraken";

        private readonly string _krakenBaseUrl;

        private const string MethodTag = "{method}";
        private const string Endpoint = $"/{MethodTag}";

        private const string UserBalanceMethod = "0/private/Balance";
        private const string TickerInformationMethod = "0/public/Ticker";
        private const string AssetInformationMethod = "0/public/Assets";

        private const string NonceKey = "nonce";
        private const string ApiKey = "API-Key";
        private const string ApiSign = "API-Sign";

        private const string ContentTypeFormat = "application/json";
        private const string PairDefaultCurrency = "USD";
        private const string PairDefaultStableCurrency = "USDT";

        private readonly Dictionary<string, string> _cryptoSymbolEquivalence = new()
        {
            ["XDG"] = "DOGE"
        };

        public KrakenService(IHttpClientFactory httpClient, IConfiguration configuration) : base(httpClient)
        {
            _krakenBaseUrl = configuration.GetValue<string>(ConfigBaseUrlIndex) ?? DefaultBaseUrl;
        }

        public async Task<IEnumerable<PortfolioData>?> GetUserBalanceAsync(string ApiKey, string ApiSecret)
        {
            var userBalanceResponse = await GetUserBalance(ApiKey, ApiSecret);

            if(userBalanceResponse?.result is not null)
            {
                return ParseUserBalance(
                    userBalanceResponse, 
                    await GetPairsTicker(), 
                    await GetAssetsInfo());
            }

            return null;
        }

        private async Task<Dictionary<string, string>> GetAssetsInfo()
        {
            var result = new Dictionary<string, string>();

            var endPoint = Endpoint.Replace(MethodTag, AssetInformationMethod);

            var assetsInfoResponse = await SendAsync<AssetsInfoResponse>(new ApiRequest()
            {
                ApiType = ApiType.GET,
                Url = _krakenBaseUrl + endPoint,
                HeaderDictionary = new Dictionary<string, string>()
                {
                    ["Accept"] = MediaTypes.JsonMediaType
                }
            });

            if(assetsInfoResponse is not null)
            {
                foreach(var asset in assetsInfoResponse.result)
                {
                    result.Add(asset.Key, asset.Value.altname);
                }
            }

            return result;
        }

        private async Task<Dictionary<string,decimal>> GetPairsTicker()
        {
            var result = new Dictionary<string,decimal>();  

            var endPoint = Endpoint.Replace(MethodTag, TickerInformationMethod);

            var tickerInfoResponse = await SendAsync<TickerInfoResponse>(new ApiRequest()
            {
                ApiType = ApiType.GET,
                Url = _krakenBaseUrl + endPoint,
                HeaderDictionary = new Dictionary<string, string>()
                {
                    ["Accept"] = MediaTypes.JsonMediaType
                }
            });

            if(tickerInfoResponse is not null)
            {
                foreach (var pair in tickerInfoResponse.result)
                {
                    result.Add(pair.Key,
                        decimal.Parse(pair.Value.c[0]));
                }
            }

            return result;
        }

        private IEnumerable<PortfolioData> ParseUserBalance(
            UserBalanceResponse userBalanceResponse, 
            Dictionary<string, decimal> pairsPrice, 
            Dictionary<string, string> assetsInfo)
        {
            List<PortfolioData> portfolioDatas = new List<PortfolioData>();

            foreach (var instrument in userBalanceResponse.result.Keys)
            {
                decimal value = 0;

                string cryptoCurrencySymbol = 
                    assetsInfo.ContainsKey(instrument) ?
                    assetsInfo[instrument] :
                    instrument;

                var balance = userBalanceResponse.result[instrument];

                if (!(balance > 0))
                    continue;

                if (cryptoCurrencySymbol.Contains("USD"))
                {
                    value = balance;
                }
                else
                {
                    if (pairsPrice.ContainsKey(cryptoCurrencySymbol + PairDefaultCurrency))
                    {
                        value = balance * pairsPrice[cryptoCurrencySymbol + PairDefaultCurrency];
                    }
                    else if(pairsPrice.ContainsKey(cryptoCurrencySymbol + PairDefaultStableCurrency))
                    {
                        value = balance * pairsPrice[cryptoCurrencySymbol + PairDefaultStableCurrency];
                    }
                    else
                    {
                        throw new Exception($"The pairs {cryptoCurrencySymbol + PairDefaultCurrency} and +" +
                                            $"{cryptoCurrencySymbol + PairDefaultStableCurrency} does not exists in Kraken tickers list");
                    }
                   
                }

                if(value == 0)
                    continue;

                portfolioDatas.Add(new PortfolioData()
                {
                    AssetSourceName = AssetSourceNames.CryptoHolding,
                    AssetPlatformName = CryptoExchangeNames.Kraken,
                    Value = value,
                    FiatCurrencyCode = PairDefaultCurrency,
                    CryptoCurrencySymbol = 
                        _cryptoSymbolEquivalence.ContainsKey(cryptoCurrencySymbol) ?
                        _cryptoSymbolEquivalence[cryptoCurrencySymbol] :
                        cryptoCurrencySymbol
                    ,
                    Quantity = balance
                }) ;
                
            }

            return portfolioDatas;

        }

        private async Task<UserBalanceResponse?> GetUserBalance(string apiKey, string apiSecret)
        {
            Dictionary<string, string> headersParameters = new ();

            var nonce = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

            var postData = NonceKey + "=" + nonce;

            var endpoint = Endpoint
                .Replace(MethodTag, UserBalanceMethod);

            headersParameters[ApiKey] = apiKey;
            headersParameters[ApiSign] = GetSign(nonce, postData, apiSecret, endpoint);

            return await SendAsync<UserBalanceResponse>(new ApiRequest()
            {
                ApiType = ApiType.POST,
                Url = _krakenBaseUrl + endpoint,
                HeaderDictionary = headersParameters,
                Data = postData,
                MediaType = MediaTypes.WwwFormMediaType
            }) ; 
        }
        private string GetSign(string nonce, string postData, string apiSecret, string uriPath)
        {
            var np = nonce + postData;

            var pathBytes = Encoding.UTF8.GetBytes(uriPath);
            var npBytes = Encoding.UTF8.GetBytes(np);
            var secretBytes = Convert.FromBase64String(apiSecret);

            using (var sha256 = SHA256.Create())
            using (var hmac = new HMACSHA512(secretBytes))
            {
                var hash256 = sha256.ComputeHash(npBytes);
                var allBytes = new byte[pathBytes.Length + hash256.Length];
                pathBytes.CopyTo(allBytes, 0);
                hash256.CopyTo(allBytes, pathBytes.Length);

                var signature = hmac.ComputeHash(allBytes);
                return Convert.ToBase64String(signature);
            }
        }

    }
}
