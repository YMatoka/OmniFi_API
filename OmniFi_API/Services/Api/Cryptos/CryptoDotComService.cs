using OmniFi_API.DTOs.CryptoDotCom;
using OmniFi_API.Services.Interfaces;
using OmniFi_API.Models.Api;
using OmniFi_API.Models.Api.CryptoDotCom;
using System.Security.Cryptography;
using System.Text;
using NuGet.Common;
using static OmniFi_API.Utilities.ApiTypes;
using OmniFi_API.Models.Portfolio;
using OmniFi_API.Utilities;
using Newtonsoft.Json;
using System.Net.Http;
using Azure.Core;
using OmniFi_API.Services.Api;
using System.Globalization;

namespace OmniFi_API.Services.Api.Cryptos
{
    public class CryptoDotComService : BaseService, IFinancialAssetService
    {
        private const string DefaultBaseUrl = "https://api.crypto.com/exchange";
        private const string ConfigBaseUrlIndex = "CryptoApiBaseUrls:CryptoDotCom";

        private readonly string _cryptoDotComBaseUrl;

        private const string VersionTag = "{version}";
        private const string MethodTag = "{method}";
        private const string Endpoint = $"/{VersionTag}/{MethodTag}";

        private const string UserBalanceMethod = "private/user-balance";
        private const string ApiVersion = "v1";

        private const string DefaultCurrency = FiatCurrencyCodes.USD;

        private const string AcceptTypeFormatKey = "Accept";
        private const string AcceptTypeFormat = MediaTypes.JsonMediaType;

        public CryptoDotComService(IHttpClientFactory httpClient, IConfiguration configuration) : base(httpClient)
        {
            _cryptoDotComBaseUrl = configuration.GetValue<string>(ConfigBaseUrlIndex) ?? DefaultBaseUrl;
        }

        public async Task<IEnumerable<PortfolioData>?> GetUserBalanceAsync(
            string apiKey, 
            string apiSecret, 
            string? accountId = null, 
            string? platformName = null)
        {
            var userBalanceResponse = await GetUserBalance(apiKey, apiSecret);

            return userBalanceResponse?.result is not null ?
                ParseUserBalance(userBalanceResponse) :
                null;
        }

        private IEnumerable<PortfolioData> ParseUserBalance(UserBalanceResponse userBalanceResponse)
        {
            List<PortfolioData> portfolioDatas = new List<PortfolioData>();

            foreach (var balance in userBalanceResponse.result.data)
            {
                foreach (var position in balance.position_balances)
                {
                    portfolioDatas.Add(new PortfolioData()
                    {
                        AssetSourceName = AssetSourceNames.CryptoHolding,
                        AssetPlatformName = CryptoExchangeNames.CryptoDotCom,
                        Balance = decimal.Parse(position.market_value, CultureInfo.InvariantCulture),
                        FiatCurrencyCode = DefaultCurrency,
                        CryptoCurrencySymbol = position.instrument_name,
                        Quantity = decimal.Parse(position.quantity, CultureInfo.InvariantCulture)
                    });
                }
            }

            return portfolioDatas;

        }

        private async Task<UserBalanceResponse?> GetUserBalance(string apiKey, string apiSecret)
        {
            var cryptoDotComRequest = new CryptoDotComRequest()
            {
                id = new Random().Next(1000, 9999),
                method = UserBalanceMethod,
                api_key = apiKey,
                nonce = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()
            };

            cryptoDotComRequest.sig = GetSign(cryptoDotComRequest, apiKey, apiSecret);

            return await SendAsync<UserBalanceResponse>(new ApiRequest()
            {
                ApiType = ApiType.POST,

                Url = _cryptoDotComBaseUrl + Endpoint
                .Replace(VersionTag, ApiVersion)
                .Replace(MethodTag, UserBalanceMethod),

                Data = cryptoDotComRequest
            });
        }

        private string GetSign(CryptoDotComRequest request, string apiKey, string apiSecret)
        {
            string paramString = string.Join("",
                request.@params.Keys.OrderBy(key => key)
                .Select(key => key + request.@params[key]));

            string sigPayLoad = request.method + request.id + apiKey + paramString + request.nonce;

            var hash = new HMACSHA256(Encoding.UTF8.GetBytes(apiSecret));

            var computedHash = hash.ComputeHash(Encoding.UTF8.GetBytes(sigPayLoad));

            return BitConverter.ToString(computedHash).Replace("-", "").ToLower();
        }

    }
}
