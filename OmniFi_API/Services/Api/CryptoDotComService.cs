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

namespace OmniFi_API.Services.Api
{
    public class CryptoDotComService : BaseService, ICryptoDotComService
    {
        private const string DefaultBaseUrl = "https://api.crypto.com/exchange";
        private const string ConfigBaseUrlIndex = "ApiBaseUrls:CryptoDotCom";


        private readonly string _cryptoDotComBaseUrl;

        private const string VersionTag = "{version}";
        private const string MethodTag = "{method}";
        private const string Endpoint = $"/{VersionTag}/{MethodTag}";

        private const string UserBalanceMethod = "private/user-balance";
        private const string ApiVersion = "v1";


        public CryptoDotComService(IHttpClientFactory httpClient, IConfiguration configuration) : base(httpClient)
        {
            _cryptoDotComBaseUrl = configuration.GetValue<string>(ConfigBaseUrlIndex) ?? DefaultBaseUrl;
        }

        public async Task<IEnumerable<PortfolioData>?> GetUserBalanceAsync(string apiKey, string apiSecret)
        {
            var userBalanceResponse = await GetUserBalance(apiKey, apiSecret);

            return userBalanceResponse is not null ? 
                ParseUserBalance(userBalanceResponse) : 
                null;
        }

        private IEnumerable<PortfolioData> ParseUserBalance(UserBalanceResponse userBalanceResponse)
        {
            List<PortfolioData> portfolioDatas = new List<PortfolioData>();

            foreach ( var balance in userBalanceResponse.result.data)
            {
                foreach(var position in balance.position_balances)
                {
                    portfolioDatas.Add(new PortfolioData()
                    {
                        AssetSourceName = AssetSourceNames.CryptoHolding,
                        AssetPlatformName = CryptoExchangeNames.CryptoDotCom,
                        Value = decimal.Parse(position.market_value),
                        FiatCurrencyCode = balance.instrument_name,
                        CryptoCurrencySymbol = position.instrument_name,
                        Quantity = decimal.Parse(position.quantity)
                    });
                }
            }

            return portfolioDatas;

        }

        private async Task<UserBalanceResponse?> GetUserBalance(string apiKey, string apiSecret)
        {
            var cryptoDotComRequest = new CryptoDotComRequest()
            {
                Id = 7,
                Method = UserBalanceMethod,
                Api_key = apiKey,
                Nonce = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };

            cryptoDotComRequest.Sig = GetSign(cryptoDotComRequest, apiKey, apiSecret);

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
                request.Params.Keys.OrderBy(key => key)
                .Select(key => key + request.Params[key]));

            string sigPayLoad = request.Method + request.Id + apiKey + paramString + request.Nonce;

            var hash = new HMACSHA256(Encoding.Unicode.GetBytes(apiSecret));

            var computedHash = hash.ComputeHash(Encoding.Unicode.GetBytes(sigPayLoad));

            return Convert.ToHexString(computedHash);
        }
    }
}
