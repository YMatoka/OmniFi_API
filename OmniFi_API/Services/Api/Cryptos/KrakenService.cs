
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

namespace OmniFi_API.Services.Api.Cryptos
{
    public class KrakenService : BaseService, IKrakenService
    {
        private const string DefaultBaseUrl = "https://api.kraken.com/0";
        private const string ConfigBaseUrlIndex = "CryptoApiBaseUrls:Kraken";

        private readonly string _krakenBaseUrl;

        private const string MethodTag = "{method}";
        private const string Endpoint = $"/{MethodTag}";

        private const string UserBalanceMethod = "/0/private/BalanceEx";

        private const string NonceKey = "nonce";
        private const string ApiKey = "API-Key";
        private const string ApiSign = "API-Sign";

        public KrakenService(IHttpClientFactory httpClient, IConfiguration configuration) : base(httpClient)
        {
            _krakenBaseUrl = configuration.GetValue<string>(ConfigBaseUrlIndex) ?? DefaultBaseUrl;
        }

        public async Task<IEnumerable<PortfolioData>?> GetUserBalanceAsync(string ApiKey, string ApiSecret)
        {
            var userBalanceResponse = await GetUserBalance(ApiKey, ApiSecret);

            return userBalanceResponse?.result is not null ?
                ParseUserBalance(userBalanceResponse) :
                null;
        }

        private IEnumerable<PortfolioData> ParseUserBalance(UserBalanceResponse userBalanceResponse)
        {
            List<PortfolioData> portfolioDatas = new List<PortfolioData>();

            foreach (var instrument in userBalanceResponse.result.Keys)
            {

                var balance = userBalanceResponse.result[instrument];

                portfolioDatas.Add(new PortfolioData()
                {
                    AssetSourceName = AssetSourceNames.CryptoHolding,
                    AssetPlatformName = CryptoExchangeNames.Kraken,
                    Value = balance.balance,
                    FiatCurrencyCode = instrument,
                    CryptoCurrencySymbol = instrument,
                    Quantity = balance.hold_trade
                });
                
            }

            return portfolioDatas;

        }

        private async Task<UserBalanceResponse?> GetUserBalance(string apiKey, string apiSecret)
        {
            Dictionary<string, string> headersParameters = new ();

            var nonce = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            Dictionary<string,string> requestData = new ()
            {
                [NonceKey] = nonce.ToString()
            };

            Dictionary<string, long> requestData2 = new()
            {
                [NonceKey] = nonce
            };

            var endpoint = Endpoint
                .Replace(MethodTag, UserBalanceMethod);

            headersParameters[ApiKey] = apiKey;
            headersParameters[ApiSign] = GetSign(requestData, apiKey, apiSecret, endpoint);

            return await SendAsync<UserBalanceResponse>(new ApiRequest()
            {
                ApiType = ApiType.POST,
                Url = _krakenBaseUrl + endpoint,
                HeaderDictionary = headersParameters,
                Data = requestData2
            }); 
        }

        private string GetSign(Dictionary<string, string> request, string apiKey, string apiSecret, string url)
        {

            string postData = string.Join("",
                request.Select(x => x!.Key + "=" + x.Value));

            byte[] nonceAndPostData = Encoding.UTF8.GetBytes(request[NonceKey] + postData);

            using (var sha256 = SHA256.Create())
            {
                var hash256Bytes = sha256.ComputeHash(nonceAndPostData);
                var hash256Str = Encoding.UTF8.GetString(hash256Bytes);

                var pathBytes = Encoding.UTF8.GetBytes(url + hash256Str);

                using (var hmacsha512 = new HMACSHA512(Convert.FromBase64String(apiSecret)))
                {
                    var hash512Bytes = hmacsha512.ComputeHash(pathBytes);
                    return Convert.ToBase64String(hash512Bytes);
                }
            }

        }

    }
}
