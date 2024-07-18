
using Microsoft.AspNetCore.DataProtection;
using OmniFi_API.DTOs.Kraken;
using OmniFi_API.Models.Api;
using OmniFi_API.Models.Portfolio;
using OmniFi_API.Services.Interfaces;
using static OmniFi_API.Utilities.ApiTypes;
using System.Security.Cryptography;
using System.Text;
using OmniFi_API.Utilities;

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

            return userBalanceResponse is not null ?
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

            Dictionary<string,string> requestData = new ()
            {
                [NonceKey] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()
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
                Data = requestData
            }); 
        }

        private string GetSign(Dictionary<string, string> request, string apiKey, string apiSecret, string url)
        {

            string postData = string.Join("",
                request.Select(x => x!.Key + "=" + x.Value));

            byte[] decodedSecret = Convert.FromBase64String(apiSecret);
            byte[] pathBytes = Encoding.UTF8.GetBytes(url);

            byte[] nonceAndPostData = Encoding.UTF8.GetBytes(request[NonceKey] + postData);

            byte[] hash256 = SHA256.Create().ComputeHash(nonceAndPostData);

            byte[] z = new byte[pathBytes.Length + hash256.Length];

            pathBytes.CopyTo(z, 0);
            hash256.CopyTo(z, pathBytes.Length);

            using (var hmac = new HMACSHA512(decodedSecret))
            {
                byte[] signature = hmac.ComputeHash(z);
                return Convert.ToBase64String(signature);
            }
        }

    }
}
